﻿using KSP.Game;
using SpaceWarp.API.Mods.JSON;
using UnityEngine;

namespace SpaceWarp.UI;

public class ModListUI : KerbalMonoBehaviour
{
    private static bool _loaded;

    private bool _drawUI;
    private Rect _windowRect;
    private ModInfo _selectedMetaData;

    private int _windowWidth = 350;
    private int _windowHeight = 700;

    private static GUIStyle _boxStyle;
    private static Vector2 _scrollPositionMods;
    private static Vector2 _scrollPositionInfo;
    private static GUIStyle _closeButtonStyle;
    private static GUIStyle _outdatedModStyle;
    
    private const string ModListHeader = "ModListHeader";

    public void Start()
    {
        if (_loaded)
        {
            Destroy(this);
        }

        _loaded = true;
    }

    private void Awake()
    {
        float minResolution = 1280f / 720f; 
        float maxResolution = 2048f / 1080f;
        float screenRatio = (float) Screen.width / (float) Screen.height;
        float scaleFactor = Mathf.Clamp(screenRatio, minResolution, maxResolution);

        _windowWidth = (int) (Screen.width * 0.5f * scaleFactor);
        _windowHeight = (int) (Screen.height * 0.5f * scaleFactor);
        _windowRect = new Rect(
            Screen.width * 0.15f,
            Screen.height * 0.15f,
            Screen.width * 0.5f * scaleFactor,
            Screen.height * 0.5f * scaleFactor
        );
    }

    private void OnGUI()
    {
        GUI.skin = SpaceWarpManager.Skin;
        if (!_drawUI)
        {
            return;
        }

        _closeButtonStyle ??= new GUIStyle(GUI.skin.button)
        {
            fontSize = 8
        };

        if (_outdatedModStyle != null)
        {
            _outdatedModStyle = new GUIStyle(GUI.skin.button)
            {
                normal =
                {
                    textColor = Color.yellow
                },
                active =
                {
                    textColor = Color.yellow
                },
                hover =
                {
                    textColor = Color.yellow
                },
                focused =
                {
                    textColor = Color.yellow
                },
                onActive =
                {
                    textColor = Color.yellow
                },
                onFocused =
                {
                    textColor = Color.yellow
                },
                onHover =
                {
                    textColor = Color.yellow
                },
                onNormal =
                {
                    textColor = Color.yellow
                }
            };
        }

        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        GUILayoutOption width = GUILayout.Width((float)(_windowWidth * 0.8));
        GUILayoutOption height = GUILayout.Height((float)(_windowHeight * 0.8));

        _windowRect = GUILayout.Window(controlID, _windowRect, FillWindow, ModListHeader, width, height);
    }
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.M))
        {
            ToggleVisible();
        }
        
        if (Input.GetKey(KeyCode.Escape) && _drawUI)
        {
            CloseWindow();
            GUIUtility.ExitGUI();
        }
    }

    private void FillWindow(int windowID)
    {
        _boxStyle = GUI.skin.GetStyle("Box");
        if (GUI.Button(new Rect(_windowRect.width - 18, 2, 16, 16), "x", _closeButtonStyle))
        {
            _drawUI = false;
            GUIUtility.ExitGUI();
        }

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();

        _scrollPositionMods = GUILayout.BeginScrollView(
            _scrollPositionMods, 
            false, 
            false,
            GUILayout.Height((float)(_windowHeight * 0.8)), 
            GUILayout.Width(300)
        );
        
        foreach (var mod in SpaceWarpManager.SpaceWarpPlugins)
        {
            if (SpaceWarpManager.ModsOutdated[mod.SpaceWarpMetadata.ModID])
            {
                if (GUILayout.Button(mod.SpaceWarpMetadata.Name, _outdatedModStyle))
                {
                    _selectedMetaData = mod.SpaceWarpMetadata;
                }
            }
            else
            {
                if (GUILayout.Button(mod.SpaceWarpMetadata.Name))
                {
                    _selectedMetaData = mod.SpaceWarpMetadata;
                }
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        if (_selectedMetaData != null)
        {
            GUILayout.BeginVertical();
            _scrollPositionInfo = GUILayout.BeginScrollView(_scrollPositionInfo, false, false);
            GUILayout.Label($"{_selectedMetaData.Name} (id: {_selectedMetaData.ModID})");
            GUILayout.Label($"Author: {_selectedMetaData.Author}");
            GUILayout.Label(SpaceWarpManager.ModsOutdated[_selectedMetaData.ModID]
                ? $"Version: {_selectedMetaData.Version} (outdated)"
                : $"Version: {_selectedMetaData.Version}");
            GUILayout.Label($"Source: {_selectedMetaData.Source}");
            GUILayout.Label($"Description: {_selectedMetaData.Description}");
            GUILayout.Label($"KSP2 Version: {_selectedMetaData.SupportedKsp2Versions.Min} - {_selectedMetaData.SupportedKsp2Versions.Max}");
            GUILayout.Label($"Dependencies");

            foreach (DependencyInfo dependency in _selectedMetaData.Dependencies)
            {
                GUILayout.Label($"{dependency.ID}: {dependency.Version.Min} - {dependency.Version.Max}");
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Open Configuration Manager"))
        {
            SpaceWarpManager.ConfigurationManager.DisplayingWindow =
                !SpaceWarpManager.ConfigurationManager.DisplayingWindow;
            _drawUI = false;
        }
        GUILayout.EndVertical();
        GUI.DragWindow();
    }

    public void ToggleVisible()
    {
        _drawUI = !_drawUI;
    }
    
    public void CloseWindow()
    {
        ToggleVisible();
    }
    
}