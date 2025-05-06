using System.Text;
using UnityEngine;

public class GameConsoleWithScrollView : MonoBehaviour
{
    [SerializeField] private bool _showOutput = true;
    [SerializeField] private bool _showStack = false;
    [SerializeField] private Rect _posRect = new Rect(50, 75 + 50, 800, 800);
    [SerializeField] private Rect _viewRect = new Rect(0, 0, 800, 60000);
    [SerializeField] private bool _show = false;

    private Vector2 _scrollPos;
    private int _errorCount = 0;
    private StringBuilder _stringInfo = new StringBuilder();

    public static GameConsoleWithScrollView Instance;

    void Awake()
    {
#if DEVELOPMENT_BUILD
        Instance = this;
        _stringInfo.AppendLine("CONSOLE:");
#else
        Destroy(gameObject);
#endif
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            _show = !_show;
            Debug.Log("~");
        }
    }

    void OnEnable()
    {
        Application.RegisterLogCallback(HandleLog);
    }

    void OnDisable()
    {
        Application.RegisterLogCallback(null);
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error || type == LogType.Log)
            _errorCount++;

        if (_showOutput && _showStack)
        {
            _stringInfo.Append("\n");

            if (_showOutput)
                _stringInfo.AppendLine(logString);
        }
    }

    public void OnGUI()
    {
        if (_show)
        {
            GUI.Label(new Rect(_posRect.x, _posRect.y - 20, 200, 50), "[errors " + _errorCount + "] length: " + _stringInfo.Length);

            _scrollPos = GUI.BeginScrollView(_posRect, _scrollPos, _viewRect);
            GUIStyle myStyle = new GUIStyle(GUI.skin.textField);
            myStyle.fontSize = 30;
            myStyle.wordWrap = true;
            GUI.TextArea(new Rect(0, 0, _viewRect.width - 50, _viewRect.height), _stringInfo.ToString(), myStyle);
            GUI.EndScrollView();
        }
    }
}
