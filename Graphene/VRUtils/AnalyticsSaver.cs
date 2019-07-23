using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Graphene.VRUtils
{
    [Serializable]
    class Analytic
    {
        public string key;
        public string name;
        public string value;
        public float time;

        public Analytic()
        {
        }

        public Analytic(string key, string name, string value, float time)
        {
            this.key = key;
            this.name = name;
            this.value = value;
            this.time = time;
        }
    }

    [Serializable]
    class Session
    {
        public string key;
        public string name;

        public List<Analytic> data;

        public Session()
        {
        }

        public Session(string key, string name)
        {
            this.key = key;
            this.name = name;
            data = new List<Analytic>();
        }

        public void AddData(Analytic analytic)
        {
            data.Add(analytic);
        }
    }

    public static class AnalyticsSaver
    {
        private static float _iniTime;

        private static List<Session> _sessions;

        public static void ResetTime(string key, string name)
        {
            if (_sessions == null)
            {
                RestoreLocalData();

                if (_sessions == null)
                    _sessions = new List<Session>();
            }

            _sessions.Add(new Session(key, name));

            _iniTime = Time.realtimeSinceStartup;

            Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin, key, name);
        }

        public static void SaveData(string key, string action, string value)
        {
            var t = Time.realtimeSinceStartup - _iniTime;

            if (_sessions != null && _sessions.Count > 0)
                _sessions.Last().AddData(new Analytic(key, action, value, t));

            Firebase.Analytics.FirebaseAnalytics.LogEvent("key", action + "_" + value, t);
        }

        public static void SaveToDisk()
        {
            //%userprofile%\AppData\Local\Packages\<productname>\LocalState.

            var path = $"{Application.dataPath}/../analytic_data.json";

            File.WriteAllText(path, JsonConvert.SerializeObject(_sessions));
        }

        public static void RestoreLocalData()
        {
            var path = $"{Application.dataPath}/../analytic_data.json";

            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                _sessions = JsonConvert.DeserializeObject<List<Session>>(json);
            }
        }
    }
}