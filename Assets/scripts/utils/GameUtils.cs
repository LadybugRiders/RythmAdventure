using UnityEngine;
using System.Collections;

public class GameUtils {

    public static GameObject CreateCharacterUIObject(string _id, float _scale, bool _draggable = false)
    {
        var charaData = ProfileManager.instance.GetCharacter(_id);
        return CreateCharacterUIObject(charaData, _scale, _draggable);
    }
    /// <summary>
    /// Create an gameobject of a character for UI purposes ( Image instead of Sprite )
    /// </summary>
    public static GameObject CreateCharacterUIObject(ProfileManager.CharacterData _charaData, float _scale, bool _draggable = false)
    {
        //Create character
        GameObject character = DataManager.instance.CreateCharacter(_charaData);
        character.name = _charaData.Id;
        //convert to ui
        Utils.SetLayerRecursively(character, LayerMask.NameToLayer("SpriteUI"));
        Utils.ConvertToUIImage(character);
        //Set Parent
        GameObject container = new GameObject("Char_" + _charaData.Id);
        Utils.SetLocalScaleXY(character.transform, _scale, _scale);
        var rect = container.AddComponent<RectTransform>();
        character.transform.SetParent(container.transform, false);
        //Set Draggable
        var uiItem = container.AddComponent<UIInventoryDraggableItem>();
        uiItem.IsDraggable = _draggable;
        uiItem.ItemParentTransform = character.transform;
        uiItem.CharId = _charaData.Id;
        return container;
    }

}
