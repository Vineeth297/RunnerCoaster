using System.Collections;
using System.Collections.Generic;
using OneSignalSDK;
using UnityEngine;

public class OneSignalScript : MonoBehaviour
{
    void Start()
    {
        OneSignal.Default.Initialize("31129411-8e01-4e7b-b389-59f2145d8870");
        OneSignal.Default.SetExternalUserId("123456789");
    }
}
