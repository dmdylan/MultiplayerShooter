using Mirror;
using TMPro;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerNameText = null;
    [SerializeField] private GameObject floatingInfo = null;
    private Material playerMaterialClone;

    [SyncVar(hook = nameof(OnNameChanged))]
    private string playerName;

    [SyncVar(hook = nameof(OnColorChanged))]
    private Color playerColor = Color.white;

    void OnNameChanged(string _old, string _new)
    {
        playerNameText.text = playerName;
    }

    void OnColorChanged(Color _old, Color _new)
    {
        playerNameText.color = _new;
        playerMaterialClone = new Material(GetComponent<Renderer>().material)
        {
            color = _new
        };
        GetComponent<Renderer>().material = playerMaterialClone;
    }

    public override void OnStartLocalPlayer()
    {
        //floatingInfo.transform.localPosition = new Vector3(0, -0.3f, 0.6f);
        //floatingInfo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        CmdSetupPlayer("Player" + Random.Range(100, 999), new Color(Random.Range(0f, 1f),
        Random.Range(0f, 1f), Random.Range(0f, 1f)));
    }

    [Command]
    public void CmdSetupPlayer(string _name, Color _col)
    {
        //player info sent to server, then server updates sync vars which handles it on all clients
        playerName = _name;
        playerColor = _col;
    }

    // Update is called once per frame
    void Update()
    {
        //allow all players to run this
        if (isLocalPlayer == false)
        {
            floatingInfo.transform.LookAt(Camera.main.transform);
        }
    }
}
