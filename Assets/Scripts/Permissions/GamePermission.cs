using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
public class GamePermission : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // if(HasPermissionToUseAR()==false)
        // {
        //     RequestPermission();
        // }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static bool HasPermissionToUseAR()
    {
    #if PLATFORM_ANDROID
        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            return true;
        }
        return false;
    #else
        return true;
    #endif
        
    }

    public static void RequestPermission()
    {
    #if PLATFORM_ANDROID
        Permission.RequestUserPermission(Permission.Camera);
    #endif
    }
}
