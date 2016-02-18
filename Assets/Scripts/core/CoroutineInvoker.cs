using System.Collections;
using UnityEngine;

namespace DevilMind
{
   public class CoroutineInvoker : MonoBehaviour
   {
       #region Singleton Object

       private static CoroutineInvoker _instance;

       public static CoroutineInvoker Instance
       {
           get
           {
               if (_instance == null)
               {
                   _instance = new CoroutineInvoker();
               }
               return _instance;
           }
       }

       #endregion

       public void Awake()
       {
           DontDestroyOnLoad(this);
           _instance = GetComponent<CoroutineInvoker>();
       }

       private IEnumerator NextFrame(System.Action callback)
       {
           yield return new WaitForEndOfFrame();
           callback();
       }

       private IEnumerator WaitForFixedUpdate(System.Action callback)
       {
           yield return new WaitForFixedUpdate();
           callback();
       }

       public static void Run(IEnumerator coroutine)
       {
           if (_instance == null)
           {
               Log.Error(MessageGroup.Common, "no coroutine invoker");
               return;
           }
           _instance.StartCoroutine(coroutine);
       }

       public static void RunNextFrame(System.Action callback)
       {
           if (_instance == null)
           {
               Log.Error(MessageGroup.Common, "no coroutine invoker");
               return;
           }
           _instance.StartCoroutine(_instance.NextFrame(callback));
       }

       public static void RunInFixedUpdate(System.Action callback)
       {
           if (_instance == null)
           {
               Log.Error(MessageGroup.Common, "no coroutine invoker");
               return;
           }
           _instance.StartCoroutine(_instance.WaitForFixedUpdate(callback));
       }
   }
}