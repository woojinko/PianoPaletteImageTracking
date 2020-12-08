using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

/// This component listens for images detected by the <c>XRImageTrackingSubsystem</c>
/// and overlays some information as well as the source Texture2D on top of the
/// detected image.
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
[RequireComponent(typeof(ARSessionOrigin))]
public class TrackedImageInfoManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The camera to set on the world space UI canvas for each instantiated image info.")]
    Camera m_WorldSpaceCanvasCamera;

    /// <summary>
    /// The prefab has a world space UI canvas,
    /// which requires a camera to function properly.
    /// </summary>
    public Camera worldSpaceCanvasCamera
    {
        get { return m_WorldSpaceCanvasCamera; }
        set { m_WorldSpaceCanvasCamera = value; }
    }

    [SerializeField]
    [Tooltip("If an image is detected but no source texture can be found, this texture is used instead.")]
    Texture2D m_DefaultTexture;

    /// <summary>
    /// If an image is detected but no source texture can be found,
    /// this texture is used instead.
    /// </summary>
    public Texture2D defaultTexture
    {
        get { return m_DefaultTexture; }
        set { m_DefaultTexture = value; }
    }

    [SerializeField]
    [Tooltip("The song to play.")]
    AudioSource m_SongAudioSource;

    public AudioSource songAudioSource
    {
        get { return m_SongAudioSource; }
        set { m_SongAudioSource = value; }
    }

    // [SerializeField]
    // [Tooltip("The prefab array to instantiate")]
    public GameObject[] animationPrefabArray;

    // public List<GameObject> AnimationPrefabArray
    // {
    //   get { return animationPrefabArray; }
    //   set { animationPrefabArray = value; }
    // }

    [SerializeField]
    [Tooltip("object")]
    GameObject anim;

    [SerializeField]
    [Tooltip("object")]
    GameObject animatedObject;


    [SerializeField]
    [Tooltip("The corresponding time for each array to instantiate")]
    int[] timeIntervalCounter;

    public int[] TimeIntervalCounter
    {
      get { return timeIntervalCounter; }
      set { timeIntervalCounter = value; }
    }

    [SerializeField]
    [Tooltip("The starting index of the prefab array to instantiate")]
    int prefabCounter;
    public int PrefabCounter
    {
      get { return prefabCounter; }
      set { prefabCounter = value; }
    }

    GameObject initializedObject;

    public GameObject InitializedObject
    {
      get { return initializedObject; }
      set { initializedObject = value; }
    }

    protected ARSessionOrigin sessionOrigin { get; private set; }

    private  Timer aTimer;
    private  bool allowInitialize = false;

    ARTrackedImageManager m_TrackedImageManager;

    bool first_time = true;

    private  void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        // Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",e.SignalTime);
        allowInitialize = true;
        // PrefabCounter++;
    }

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
        sessionOrigin = GetComponent<ARSessionOrigin>();
        //Instantiate(animationPrefabArray[2], sessionOrigin.trackablesParent);
        animatedObject.SetActive(false);
    }

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        Debug.Log("on enable");
        // m_SongAudioSource.PlayDelayed(5);
        //Instantiate(animationPrefabArray[3], sessionOrigin.trackablesParent);
        foreach (var animation in animationPrefabArray) {
            animation.SetActive(false);
        }
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    //called on every frame after tracked image from ar reference library first seen
    void UpdateInfo(ARTrackedImage trackedImage)
    {

        if (first_time) {

            //PrefabCounter = 0;

            // InitializedObject = Instantiate(animationPrefabArray[0], sessionOrigin.trackablesParent) as GameObject;
            InitializedObject = animationPrefabArray[PrefabCounter];
            InitializedObject.SetActive(true);
            Debug.Log("first time");
            m_SongAudioSource.Play();
            first_time = false;


            aTimer = new System.Timers.Timer(10000);
            aTimer.Elapsed += OnTimedEvent;

            aTimer.Enabled = true;

        }


        if (allowInitialize) {
            //PrefabCounter++;
            if (PrefabCounter >= animationPrefabArray.Length) {
                aTimer.Enabled = false;
                allowInitialize = false;
                return;
            }
            //animatedObject.SetActive(true);
            allowInitialize = false;
        //   Destroy(InitializedObject);
        //   InitializedObject = Instantiate(animationPrefabArray[PrefabCounter], sessionOrigin.trackablesParent);
            InitializedObject.SetActive(false);
            PrefabCounter++;
            InitializedObject = animationPrefabArray[PrefabCounter];
            InitializedObject.SetActive(true);
            aTimer.Interval = TimeIntervalCounter[PrefabCounter] * 1000;
        }

      //return Instantiate(prefab, sessionOrigin.trackablesParent)
        // Set canvas camera
        var canvas = trackedImage.GetComponentInChildren<Canvas>();
        canvas.worldCamera = worldSpaceCanvasCamera;

        // Update information about the tracked image
        var text = canvas.GetComponentInChildren<Text>();
        text.text = string.Format(
           "{0}\ntrackingState: {1}\nGUID: {2}\nReference size: {3} cm\nDetected size: {4} cm",
           trackedImage.referenceImage.name,
           trackedImage.trackingState,
           trackedImage.referenceImage.guid,
           trackedImage.referenceImage.size * 100f,
           trackedImage.size * 100f);

        var planeParentGo = trackedImage.transform.GetChild(0).gameObject;
        var planeGo = planeParentGo.transform.GetChild(0).gameObject;

        // Disable the visual plane if it is not being tracked
        if (trackedImage.trackingState != TrackingState.None)
        {
            planeGo.SetActive(true);


            Debug.Log("every time baby");


            // The image extents is only valid when the image is being tracked
            trackedImage.transform.localScale = new Vector3(trackedImage.size.x, trackedImage.size.x, trackedImage.size.y);

            // Set the texture
            var material = planeGo.GetComponentInChildren<MeshRenderer>().material;
            material.mainTexture = (trackedImage.referenceImage.texture == null) ? defaultTexture : trackedImage.referenceImage.texture;
        }
       else
        {
           planeGo.SetActive(false);
        }
    }

    //called on every frame
    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            // Give the initial image a reasonable default scale
            //trackedImage.transform.localScale = new Vector3(0.01f, 1f, 0.01f);

            UpdateInfo(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
            UpdateInfo(trackedImage);
    }
}
