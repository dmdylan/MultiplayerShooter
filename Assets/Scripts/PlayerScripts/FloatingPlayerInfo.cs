using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingPlayerInfo : NetworkBehaviour
{
    //TODO: May be able to disable the floating info if islocalplayer so it doesn't render for client, only others
    [SerializeField] private TMP_Text playerNameText = null;
    [SerializeField] private GameObject floatingInfo = null;
    [SerializeField] private Slider healthBar = null;
    private Material playerMaterialClone;
    private PlayerInfo player;
    private Camera playerCamera = null;

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
        player = GetComponent<PlayerInfo>();
        playerCamera = GetComponentInChildren<Camera>();
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

    //TODO: Can setup an event that calls this command when it is triggered
    [Command]
    public void CmdUpdateHealthBar()
    {
        healthBar.value = player.PlayerHealth / player.MaxHealth;
    }

    //Update is called once per frame
    void Update()
    {
        //allow all players to run this
        if (isLocalPlayer == false)
        {
            floatingInfo.transform.LookAt(playerCamera.transform);
            healthBar.transform.LookAt(playerCamera.transform);
        }

        if (!isLocalPlayer)
            return;

        CmdUpdateHealthBar();
    }

    private void OnDestroy()
    {
        Destroy(playerMaterialClone);
    }
}
