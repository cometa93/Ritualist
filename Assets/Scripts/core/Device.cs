using System;
using UnityEngine;

namespace DevilMind
{
   public class Device
   {
      private static string _platformHash;
      #if !UNITY_EDITOR
      private static readonly string UNIQUE_ID = SystemInfo.deviceUniqueIdentifier;
      #else
      private static readonly string UNIQUE_ID = "_testing_unique_device_id";
      #endif

      public static string ID
      {
         get
         {
            switch (Application.platform)
            {
               case RuntimePlatform.OSXEditor:
                  _platformHash = "edit_id";
                  break;
               case RuntimePlatform.WP8Player:
                  _platformHash = "wp8_id";
                  break;
               case RuntimePlatform.Android:
                  _platformHash = "and_id";
                  break;
            }
            return _platformHash + UNIQUE_ID;
         }
      }



   }
}