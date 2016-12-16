using UnityEngine;
using System.Collections;

public class GameUtils {

    /// <summary>
    /// Create an gameobject of a character for UI purposes ( Image instead of Sprite )
    /// </summary>
    public static GameObject CreateCharacterUIObject(string _id, float _scale, bool draggable = false)
    {
        //Create character
        GameObject character = DataManager.instance.CreateCharacter(_id);
        character.name = _id;
        //convert to ui
        Utils.SetLayerRecursively(character, LayerMask.NameToLayer("SpriteUI"));
        Utils.ConvertToUIImage(character);
        //Set Parent
        GameObject container = new GameObject("Char_" + _id);
        Utils.SetLocalScaleXY(character.transform, _scale, _scale);
        var rect = container.AddComponent<RectTransform>();
        character.transform.SetParent(container.transform, false);
        //Set Draggable
        var uiItem = container.AddComponent<UIInventoryDraggableItem>();
        uiItem.IsDraggable = draggable;
        uiItem.ItemParentTransform = character.transform;
        uiItem.CharId = _id;
        return container;
    }

}
