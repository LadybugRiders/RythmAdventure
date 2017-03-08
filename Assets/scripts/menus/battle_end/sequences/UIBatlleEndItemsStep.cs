using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIBatlleEndItemsStep : UIStep {

    [SerializeField] Transform m_itemsParent;

    [SerializeField] GameObject m_shardPrefab;

    BattleData m_battleData;

    Animator m_currentAnimator;

    int m_shardIndex = -1;

    string m_state = "shards";

    public override void Launch(OnStepEndDelegate _del)
    {
        base.Launch(_del);

        m_battleData = ProfileManager.instance.BattleData;

        LaunchNextShard();
    }

    public override void Skip()
    {
        base.Skip();
    }

    protected override void UpdateStep()
    {
        base.UpdateStep();
        switch (m_state)
        {
            case "shards":
                UpdateShards();
                break;
        }
    }

    void UpdateShards()
    {
        if (m_currentAnimator != null)
        {
            var animRunning = Utils.IsAnimationStateRunning(m_currentAnimator, "appear", false);
            if (animRunning == false)
            {
                m_currentAnimator.gameObject.GetComponentInChildren<Text>().enabled = true;
                LaunchNextShard();
            }
        }
    }

    void LaunchNextShard()
    {
        m_shardIndex++;
        if( m_shardIndex >= m_battleData.Shards.Count )
        {
            Stop();
            return;
        }
        var shard = m_battleData.Shards.ElementAt(m_shardIndex);
        LaunchShard(shard.Key, shard.Value);
    }

    void LaunchShard(string _shardId, int _quantity )
    {
        //Get data and color of the shard
        var shardData = DataManager.instance.InventoryManager.GetShard(_shardId);
        var shardColor = DataManager.instance.InventoryManager.GetShardColor(_shardId);

        //instantiate the right prefab
        var go = Instantiate(m_shardPrefab);
        go.transform.SetParent(m_itemsParent,false);
        m_currentAnimator = go.GetComponent<Animator>();
        //set quantity in text
        var textComp = go.GetComponentInChildren<Text>();
        if( textComp != null)
        {
            textComp.text = ""+_quantity;
            textComp.enabled = false;
        }
        //set image
        var imgComp = go.GetComponentInChildren<Image>();
        imgComp.color = shardColor.Color;
    }
}
