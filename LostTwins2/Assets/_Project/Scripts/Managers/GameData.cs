using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    [SerializeField] private Material overlayBlack = default;
    [SerializeField] private Material aurora = default;
    [SerializeField] private Material skyGradient = default;

    [SerializeField] private Material sidesYellowBoundary = default;
    [SerializeField] private Material sidesBlueBoundary = default;
    [SerializeField] private Material sidesYellowPath = default;
    [SerializeField] private Material sidesBluePath = default;
    [SerializeField] private Material sidesBluePathTopBottom = default;
    [SerializeField] private Material godraysConnect = default;
    [SerializeField] private Material godraysDisconnect = default;
    [SerializeField] private Material patternConnect = default;
    [SerializeField] private Material patternDisconnect = default;
    [SerializeField] private Material cameraFrontConnect = default;
    [SerializeField] private Material cameraFrontDisconnect = default;
    [SerializeField] private Material musicSymbol = default;

    [SerializeField] private Color particlesColorConnect = default;
    [SerializeField] private Color particlesColorDisconnect = default;

    [SerializeField] private Transform iconTransform = default;
    [SerializeField] private MeshFilter[] musicNotes;

    public Material OverlayBlack { get { return overlayBlack; } }
    public Material Aurora { get { return aurora; } }
    public Material SkyGradient { get { return skyGradient; } }

    public Material SidesYellowBoundary { get { return sidesYellowBoundary; } }
    public Material SidesBlueBoundary { get { return sidesBlueBoundary; } }
    public Material SidesYellowPath { get { return sidesYellowPath; } }
    public Material SidesBluePath { get { return sidesBluePath; } }
    public Material SidesBluePathTopBottom { get { return sidesBluePathTopBottom; } }
    public Material GodraysConnect { get { return godraysConnect; } }
    public Material GodraysDisconnect { get { return godraysDisconnect; } }
    public Material PatternConnect { get { return patternConnect; } }
    public Material PatternDisconnect { get { return patternDisconnect; } }
    public Material CameraFrontConnect { get { return cameraFrontConnect; } }
    public Material CameraFrontDisconnect { get { return cameraFrontDisconnect; } }
    public Material MusicSymbol { get { return musicSymbol; } }

    public Color ParticlesColorConnect { get { return particlesColorConnect; } }
    public Color ParticlesColorDisconnect { get { return particlesColorDisconnect; } }

    public Transform IconTransform { get { return iconTransform; } }
    public MeshFilter[] MusicNotes { get { return musicNotes; } }

    private void Awake()
    {
        instance = this;
    }
}
