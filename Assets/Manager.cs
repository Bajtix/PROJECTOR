using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class Manager : MonoBehaviour {
    [SerializeField] private VideoPlayer m_player;

    private void Start() {
        m_player.source = VideoSource.VideoClip;
        m_player.url = "";
        m_player.Play();
        Debug.Log(m_player.length);
    }
}
