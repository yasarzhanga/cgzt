using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace jxzt {
    public class StudentSaveTextureDeletePanel : MonoBehaviour {

        public Texture2D texture;
        public string studentAnswerName;

        private void Awake() {
            if (transform.GetComponent<Toggle>() != null) {
                transform.GetComponent<Toggle>().onValueChanged.AddListener((bool ison) => {
                    if (!string.IsNullOrEmpty(studentAnswerName)) {
                        if (ison) {
                            transform.parent.GetChild(0).gameObject.SetActive(true);
                            if (!DeletePanel.readyToDeleteFileName.Contains(studentAnswerName)) { 
                                DeletePanel.readyToDeleteFileName.Add(studentAnswerName);
                            }
                        }
                        else {
                            transform.parent.GetChild(0).gameObject.SetActive(false);
                            if (DeletePanel.readyToDeleteFileName.Contains(studentAnswerName)) {
                                DeletePanel.readyToDeleteFileName.Remove(studentAnswerName);
                            }
                        }

                    }
                });
            }
        }
    }
}

