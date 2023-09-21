using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventInteractionNPC : EventInteraction
{
    [Header("Info")]
    public string npcName;
    public Sprite npcSprite;

    //    [Header("Interaction")]
    public List<EventSO> eventsList;

    public override List<EventSO> GetEventList()
    {
        if (actionList == null || actionList.Count <= 0)
            actionList = eventsList;
        return actionList;
    }

    internal override void SetNewEventList(List<EventSO> newEventList)
    {
        actionList = newEventList;
    }

    private new void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

}
