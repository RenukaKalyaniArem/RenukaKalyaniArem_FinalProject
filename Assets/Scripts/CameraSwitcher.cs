using UnityEngine;
using Unity.Cinemachine;
public class CameraSwitcher : MonoBehaviour
{
    public CinemachineCamera  thirdPersonCam;
    public CinemachineCamera  pilotCam;
    public KeyCode toggleKey = KeyCode.C;
    public int activePriority = 20;
    public int inactivePriority = 5;

    void Start() => SetMode(thirdPerson: true);

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            bool thirdLive = thirdPersonCam.Priority >= pilotCam.Priority;
            SetMode(thirdPerson: !thirdLive);
        }
    }

    public void SetMode(bool thirdPerson)
    {
        if (thirdPersonCam) thirdPersonCam.Priority = thirdPerson ? activePriority : inactivePriority;
        if (pilotCam) pilotCam.Priority = thirdPerson ? inactivePriority : activePriority;
    }
}


