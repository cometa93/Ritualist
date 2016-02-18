using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DevilMind
{
    public delegate void RequestDelegate(Request request);

    public class Request
    {
        // TEST "http://localhost:57769/api/"; 
        // DEPLOY "http://devilmind.azurewebsites.net/api/";
#if UNITY_EDITOR
        private const string GAMESERVER_URL = "http://localhost:57769/api/";
#else
        private const string GAMESERVER_URL = "http://devilmind.azurewebsites.net/api/";
#endif
        protected const string TAG_DEVICE_ID = "X-deviceId";
        protected const string TAG_AUTHORIZATION = "Authorization";
        protected const string TAG_AUTHORIZATION_TYPE = "basic ";
        protected const string TAG_CONTENT_TYPE = "Content-Type";
        protected const string TAG_TYPE_JSON = "application/json";

        protected bool _waitForServerResponse;

        private bool _error;
        private string _response;
        private Dictionary<string, object> _parsedResponse;
        protected Dictionary<string, object> Data { private set; get; }

        protected virtual void Parse(string jsonText)
        {
            Log.Warning(MessageGroup.Network, "Last request doesn't implement parsing.");
        }

        protected virtual List<string> GetParameters()
        {
            return new List<string>();
        }

        protected virtual void NotifyWhenFinished()
        {   
        }
        
        protected virtual string PostData()
        {
            return "";
        }

        private void LogRequest(string route,string postParams)
        {
            Log.Info(MessageGroup.Network, "REQUST SENT ( "+route +" ) AND DATA : \n\n" + postParams);
        }

        private WWW CreateRequest()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers[TAG_CONTENT_TYPE] = TAG_TYPE_JSON;

            string postData = PostData();
            var postBytes = string.IsNullOrEmpty(postData) ? null : Encoding.UTF8.GetBytes(postData);
            var route = GAMESERVER_URL + Route();
            var parameters = GetParameters();

            if (route.EndsWith("/") == false)
            {
                route += "/";
            }
            foreach (var routeParameter in parameters)
            {
                route += routeParameter + "/";
            }

            WWW request = new WWW(route, postBytes, headers);
            LogRequest(route,postData);
            return request;
        }

        protected virtual string Route()
        {
            return "";
        }

        public IEnumerator SendRequest()
        {
            if (_waitForServerResponse)
            {
                //TODO: BLOCK USER INPUT UNTILL REQUEST IS DONE    
            }

            WWW request = CreateRequest();
            yield return request;
            Log.Info(MessageGroup.Network, "SERVER RESPONSE WITH: " + request.text);
            CheckServerResponse(request);
        }

        public bool Success()
        {
            return !_error;
        }

        public void CheckServerResponse(WWW www)
        {
            if (www == null)
            {
                return;
            }

            if (www.error != null)
            {
                _error = true;
                Log.Error(MessageGroup.Network, "Couldn't connect to server !" + www.error);
                return;
            }

            if (string.IsNullOrEmpty(www.text))
            {
                Log.Error(MessageGroup.Network, "Server responde with empty message.");
                return;
            }


            _response = www.text;
            if (_error == false)
            {
                Parse(_response);
                NotifyWhenFinished();
            }
        }

    }
}