using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

namespace Muco {
    [System.Serializable]
    public class HandPrefabs {
        public GameObject left;
        public GameObject right;
    }

    public class PlayerGhost {
        public PlayerRemoteIndicatorBase playerIndicator;
        public NetworkTransformList handL;
        public NetworkTransformList handR;
        public Transform handsParentL;
        public Transform handsParentR;

        public bool isVisible;
        public bool leftHandHasTracking;
        public bool rightHandHasTracking;

        public List<byte> networkDataBuffer;
        public bool initialized;
        
        public bool DesTrans(ref int cursor, byte[] buffer) {
            return Serialize.DesTransGlobal(playerIndicator.transform, ref cursor, buffer);
        }

        public void SetLevel(int level) {
            playerIndicator.SetLevel(level);
        }

        public void SetColor(Color color) {
            playerIndicator.SetColor(color);
        }

        public void SetIsVisible(bool isVisible) {
            this.isVisible = isVisible;
            UpdateVisibility();
        }

        public void UpdateVisibility() {
            playerIndicator.gameObject.SetActive(isVisible);
            if (handL)
                handL.gameObject.SetActive(isVisible && leftHandHasTracking);
            if (handR)
                handR.gameObject.SetActive(isVisible && rightHandHasTracking);
        }

        public bool DesHands(ref int cursor, byte[] buffer, HandPrefabs[] deviceHandPrefabs) {
            if (cursor >= buffer.Length)
                return false;
            byte handTypeIndex = buffer[cursor++];
            if (handTypeIndex >= deviceHandPrefabs.Length) {
                Debug.Log("bad hand type");
                return false;
            }
            var handPrefabs = deviceHandPrefabs[handTypeIndex];

            if (!Serialize.DesBool(out leftHandHasTracking, ref cursor, buffer))
                return false;

            if (!Serialize.DesBool(out rightHandHasTracking, ref cursor, buffer))
                return false;

            if (handsParentL == null)
                handsParentL = new GameObject().transform;

            if (handsParentR == null)
                handsParentR = new GameObject().transform;

            if (handL == null) {
                if (handPrefabs.left == null) {
                    Debug.Log("Left Hand Prefab Unassigned");
                    return false;
                }
                var go = GameObject.Instantiate(handPrefabs.left, handsParentL);
                handL = go.GetComponent<NetworkTransformList>();
                playerIndicator.leftHandRendererReference = go.GetComponent<RendererReference>();
            }
            if (handR == null) {
                var go = GameObject.Instantiate(handPrefabs.right, handsParentR);
                handR = go.GetComponent<NetworkTransformList>();
                playerIndicator.rightHandRendererReference = go.GetComponent<RendererReference>();
            }

            Serialize.DesTransGlobal(handsParentL, ref cursor, buffer);
            Serialize.DesTransGlobal(handsParentR, ref cursor, buffer);

            if (!handL.Des(ref cursor, buffer))
                return false;
            if (!handR.Des(ref cursor, buffer))
                return false;
            
            UpdateVisibility();

            return true;
        }
        
        public void UpdateAllPlayerDataDiff(ref int cursor, byte[] diff, GhostSystem ghostSystem) {
            if (networkDataBuffer == null)
                networkDataBuffer = new List<byte>();
            
            Serialize.UpdateFromDiff(networkDataBuffer, diff, ref cursor);

            if (initialized) {
                int cursor2 = 0;
                UpdateFromBuffer(ref cursor2, networkDataBuffer.ToArray(), ghostSystem);
            }
        }

        public void UpdateAllPlayerData(ref int cursor, byte[] buffer, GhostSystem ghostSystem) {
            if (networkDataBuffer == null)
                networkDataBuffer = new List<byte>();
            else
                networkDataBuffer.Clear();
            
            var begin = cursor;
            UpdateFromBuffer(ref cursor, buffer, ghostSystem);
            
            networkDataBuffer.AddRange(buffer[begin..cursor]);
            initialized = true;
            
            int cursor2 = 0;
            UpdateFromBuffer(ref cursor2, networkDataBuffer.ToArray(), ghostSystem);
        }

        public void UpdateFromBuffer(ref int cursor, byte[] buffer, GhostSystem ghostSystem) {
            for (int i = 0; i < (int)PlayerDataType.Count; i++) {
                var dataType = (PlayerDataType)i;
                ProcessPlayerDataNotify(dataType, ref cursor, buffer, ghostSystem);
            }
        }

        public void ProcessPlayerDataNotify(PlayerDataType dataType, ref int cursor, byte[] bufferList, GhostSystem ghostSystem) {
            switch (dataType) {
                case PlayerDataType.Trans: {
                    DesTrans(ref cursor, bufferList);
                    break;
                }
                case PlayerDataType.Level: {
                    float level;
                    if (!Serialize.DesFloat(out level, ref cursor, bufferList))
                        Debug.Log("Problem");
                    SetLevel((int)level);
                    break;
                }
                case PlayerDataType.Hands: {
                    if (!DesHands(ref cursor, bufferList, ghostSystem.ghostHandPrefabs))
                        Debug.Log("Problem");
                    break;
                }
                case PlayerDataType.Color: {
                    Color color;
                    if (Serialize.DesColor(out color, ref cursor, bufferList))
                        SetColor(color);
                    else
                        Debug.Log("Problem");
                    break;
                }
                case PlayerDataType.DeviceId: {
                    int device_id;
                    if (Serialize.DesI32(out device_id, ref cursor, bufferList)) {
                        
                    }
                    else
                        Debug.Log("Problem");
                    break;
                }
                case PlayerDataType.IsVisible: {
                    bool isVisible;
                    if (Serialize.DesBool(out isVisible, ref cursor, bufferList))
                        SetIsVisible(isVisible);
                    else
                        Debug.Log("Problem");
                    break;
                }
                case PlayerDataType.DevMode: {
                    bool inDevMode;
                    if (Serialize.DesBool(out inDevMode, ref cursor, bufferList)) {

                    }
                    else
                        Debug.Log("Problem");
                    break;
                }
                case PlayerDataType.Language: {
                    int language;
                    if (Serialize.DesI32(out language, ref cursor, bufferList)) {

                    }
                    else
                        Debug.Log("Problem");
                    break;
                }
                case PlayerDataType.EnvironmentData: {
                    string envName;
                    if (Serialize.DesString(out envName, ref cursor, bufferList)) { }
                    else Debug.Log("Problem");

                    string envCode;
                    if (Serialize.DesString(out envCode, ref cursor, bufferList)) { }
                    else Debug.Log("Problem");

                    Vector3 envPos;
                    if (Serialize.DesVector3(out envPos, ref cursor, bufferList)) { }
                    else Debug.Log("Problem");
                    
                    Vector3 envEuler;
                    if (Serialize.DesVector3(out envEuler, ref cursor, bufferList)) { }
                    else Debug.Log("Problem");

                    break;

                }
                case PlayerDataType.DeviceStats: {
                    if (cursor >= bufferList.Length)
                        return;
                    var battery_status = (BatteryStatus)bufferList[cursor++];
                    float battery_level;
                    Serialize.DesFloat(out battery_level, ref cursor, bufferList);

                    float fps;
                    Serialize.DesFloat(out fps, ref cursor, bufferList);

                    float trackingConfidence;
                    Serialize.DesFloat(out trackingConfidence, ref cursor, bufferList);

                    WarningLevel warningLevel = (WarningLevel)bufferList[cursor++];
                    float temperatureLevel;
                    Serialize.DesFloat(out temperatureLevel, ref cursor, bufferList);
                    float temperatureTrend;
                    Serialize.DesFloat(out temperatureTrend, ref cursor, bufferList);

                    break;
                }
                case PlayerDataType.AudioVolume: {
                    if (cursor >= bufferList.Length)
                        return;
                    float audio_volume;
                    Serialize.DesFloat(out audio_volume, ref cursor, bufferList);
                    break;
                }
                default: {
                    Debug.Log("Unknown data type while processing notify data: " + dataType);
                    break;
                }
            }
        }
    }
}
