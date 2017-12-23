using UnityEngine;
using System.Collections;

public class TeamMember : MonoBehaviour {
    int _teamId = 0;

    public int teamID {
        get { return _teamId; }
    }

    [RPC]
    void SetTeamID(int id) {
        _teamId = id;
        MeshRenderer mySkin = this.transform.GetComponentInChildren<MeshRenderer>();
        Debug.Log(mySkin);
        if (mySkin == null) {
            Debug.LogError("Couldn't Find Mesh Renderer");
        }


        switch (_teamId) {                
            case 2:
                mySkin.material.color = new Color(.5f, 1f, .5f);
                Debug.Log("COlor");
                break;
            case 1:
                mySkin.material.color = Color.red;
                break;
            default:
                mySkin.material.color = Color.white;
                break;
        } 
    }
}
