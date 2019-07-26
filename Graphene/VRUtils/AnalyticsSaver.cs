using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using VrSafety;
using UnityEngine.Analytics;

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
        public string id;
        public string key;
        public string name;

        public List<Analytic> data;

        public Session()
        {
        }

        public Session(string key, string name)
        {
            id = Guid.NewGuid().ToString();
            
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
        //TODO all of this must be refactored
        private static float _iniTime;

        private static List<Session> _sessions;

        public static string ApiUrl = "https://us-central1-vr-security.cloudfunctions.net/webApi/pepita/v1/";
        public static string AnalyticEndpoint = "analytic";
        private static int _saveCounter;
        private static bool _saving;

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
            
            Analytics.SetUserId(key);
            //Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin, key, name);
        }

        public static void SaveData(string key, string action, string value)
        {
            var t = Time.realtimeSinceStartup - _iniTime;

            if (_sessions != null && _sessions.Count > 0)
                _sessions.Last().AddData(new Analytic(key, action, value, t));

            AnalyticsEvent.Custom(key, new Dictionary<string, object>()
            {
                {action+ "_" + value, t}
            });
            //Firebase.Analytics.FirebaseAnalytics.LogEvent("key", action + "_" + value, t);
        }

        public static void SaveToDisk()
        {
            //%userprofile%\AppData\Local\Packages\<productname>\LocalState.

            var path = $"{Application.dataPath}/../analytic_data.json";

            Analytics.FlushEvents();
            
//            File.WriteAllText(path, JsonConvert.SerializeObject(_sessions));
//
//            return;
            if (_saving)
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(_sessions));

                return;
            }

            if (string.IsNullOrEmpty(ApiUrl))
            {
                Debug.LogError("ApiUrl is null");

                File.WriteAllText(path, JsonConvert.SerializeObject(_sessions));

                return;
            }

            if (string.IsNullOrEmpty(AnalyticEndpoint))
            {
                Debug.LogError("Analytic Endpoint is null");

                File.WriteAllText(path, JsonConvert.SerializeObject(_sessions));

                return;
            }
            HttpComm.Post<string>(ApiUrl, AnalyticEndpoint + "/", JsonConvert.SerializeObject(_sessions), response =>
            {
                Debug.Log(response);
                _saving = false;
                if (!response.success)
                {
                    Debug.LogError($"Error saving:\n{response.code}\n{response.error}");

//                    _saveCounter++;
//                    if (_saveCounter >= 5)
//                    {
//                        File.WriteAllText(path, JsonConvert.SerializeObject(_sessions));
//                        return;
//                    }

                    File.WriteAllText(path, JsonConvert.SerializeObject(_sessions));
                    return;
                    //SaveToDisk();
                }
                else
                {
                    _sessions = new List<Session>();
                    File.WriteAllText(path, JsonConvert.SerializeObject(_sessions));
                    Debug.Log($"Analytic saved successfully, cleaning local data");
                }
            });
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