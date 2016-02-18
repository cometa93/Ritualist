using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevilMind
{
   delegate void ParserDelegate(Dictionary<string, object> data);
   public class RequestManager
   {
       private LinkedList<RequestInfo> RequestQueue { set; get; }
       private bool _isLocked;
       private class RequestInfo
       {
           public RequestDelegate RequestDelegate { set; get; }
           public Request Request { set; get; }
           public static RequestInfo SetRequest(Request request, RequestDelegate reqDelegate)
           {
               return new RequestInfo()
               {
                   Request = request,
                   RequestDelegate = reqDelegate
               };
           }
       }

       public RequestManager()
       {
           RequestQueue = new LinkedList<RequestInfo>();
       }

       public void SendRequest(Request request, RequestDelegate requestDelegate = null, bool forceSend = false)
       {
           if (forceSend)
           {
               RequestQueue.AddFirst(RequestInfo.SetRequest(request,requestDelegate));
               return;
           }
           RequestQueue.AddLast(RequestInfo.SetRequest(request, requestDelegate));
       }

       private void SendNext()
       {

            _isLocked = true;
            RequestInfo requestInfo = RequestQueue.First.Value;
            RequestQueue.RemoveFirst();
            CoroutineInvoker.Run(NextRestQuestion(requestInfo));
       }

       private IEnumerator NextRestQuestion(RequestInfo info)
       {

           yield return CoroutineInvoker.Instance.StartCoroutine(info.Request.SendRequest());
           if (info.RequestDelegate != null)
           {
               info.RequestDelegate(info.Request);
           }
           _isLocked = false;
       }

       public void Update()
       {
           if (!_isLocked && RequestQueue.Count > 0)
           {
               SendNext();
           }
       }

       public static void SendError(string message, RequestDelegate request )
       {
            //TODO :: LOG ERRORS
       }
    }
}