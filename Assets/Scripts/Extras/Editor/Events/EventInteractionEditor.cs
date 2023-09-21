using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EventInteractionNPC))]
public class EventInteractionEditor : Editor
{

    EventInteraction EventInteractionTarget => target as EventInteraction;

    private UIManager UIM;

    private void OnSceneGUI()
    {
        UIM = UIManager.Instance;
        Handles.color = UIM.routeSphereHandleColor;
        Vector3[] route = EventInteractionTarget.defaultRoute;
        if (route == null) return;

        for (int i = 0; i < route.Length; i++)
        {
            EditorGUI.BeginChangeCheck();

            // Create Handler
            Vector3 actualPoint = route[i] + EventInteractionTarget.InitialPosition;
            Vector3 handlePoint = Handles.FreeMoveHandle(actualPoint, Quaternion.identity, UIM.routeSphereHandleRadius, new Vector3(0.3f, 0.3f, 0.3f), Handles.SphereHandleCap);

            //Create enumeration text
            GUIStyle text = new(){
                fontStyle = FontStyle.Bold,
                fontSize = 16
            };
            text.normal.textColor = Color.black;
            Vector3 textAlign = Vector3.down * 0.3f + Vector3.right * 0.3f;
            Handles.Label(EventInteractionTarget.InitialPosition + route[i] + textAlign,
            $"{i + 1}", text);

            //Save Object
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Free Move Handler");
                EventInteractionTarget.defaultRoute[i] = handlePoint - EventInteractionTarget.InitialPosition;
            }
        }
    }
}
