using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

public partial class Utils  {

	#region LOCALPOSITION
	public static Vector3 SetLocalPositionX(Transform _t, float _newValue){
		Vector3 tmpVector = _t.localPosition;
		tmpVector.x = _newValue;
		_t.localPosition = tmpVector;
		return _t.localPosition;
	}

	public static Vector3 SetLocalPositionY(Transform _t, float _newValue){
		Vector3 tmpVector = _t.localPosition;
		tmpVector.y = _newValue;
		_t.localPosition = tmpVector;
		return _t.localPosition;
	}

	public static Vector3 SetLocalPositionZ(Transform _t, float _newValue){
		Vector3 tmpVector = _t.localPosition;
		tmpVector.z = _newValue;
		_t.localPosition = tmpVector;
		return _t.localPosition;
	}

    /// <summary>
    /// Sets the local position without the Z axis
    /// </summary>
    public static Vector3 Set2DLocalPosition(Transform _t, Vector3 position)
    {
        Vector3 tmpVector = _t.localPosition;
        tmpVector.x = position.x;
        tmpVector.y = position.y;
        _t.localPosition = tmpVector;
        return _t.localPosition;
    }
    #endregion

    #region POSITION
    public static Vector3 SetPositionX(Transform _t, float _newValue){
		Vector3 tmpVector = _t.position;
		tmpVector.x = _newValue;
		_t.position = tmpVector;
		return _t.position;
	}

	public static Vector3 SetPositionY(Transform _t, float _newValue){
		Vector3 tmpVector = _t.position;
		tmpVector.y = _newValue;
		_t.position = tmpVector;
		return _t.position;
	}

	public static Vector3 SetPositionZ(Transform _t, float _newValue){
		Vector3 tmpVector = _t.position;
		tmpVector.z = _newValue;
		_t.position = tmpVector;
		return _t.position;
	}

    /// <summary>
    /// Sets the position without the Z axis
    /// </summary>
    public static Vector3 Set2DPosition(Transform _t, Vector3 position)
    {
        Vector3 tmpVector = _t.position;
        tmpVector.x = position.x;
        tmpVector.y = position.y;
        _t.position = tmpVector;
        return _t.position;
    }
    #endregion

    #region LOCALSCALE
    public static Vector3 SetLocalScaleX(Transform _t, float _newValue){
		Vector3 tmpVector = _t.localScale;
		tmpVector.x = _newValue;
		_t.localScale = tmpVector;
		return _t.localScale;
	}

	public static Vector3 SetLocalScaleY(Transform _t, float _newValue){
		Vector3 tmpVector = _t.localScale;
		tmpVector.y = _newValue;
		_t.localScale = tmpVector;
		return _t.localScale;
	}

	#endregion

	#region

	public static Vector3 SetLocalAngleZ(Transform _t, float _newValue){
		Vector3 tmpVector = _t.localEulerAngles;
		tmpVector.z = _newValue;
		_t.localEulerAngles = tmpVector;
		return _t.localEulerAngles;
	}

	#endregion

	public static void SetAlpha(SpriteRenderer _spriteRenderer,float _alpha){
		Color color = _spriteRenderer.color;
		color.a = _alpha;
		_spriteRenderer.color = color;
	}

	public static void SetAlpha(TextMesh _textMesh,float _alpha){
		Color color = _textMesh.color;
		color.a = _alpha;
		_textMesh.color = color;
	}

	public static bool IsAnimationStateRunning( Animator _animator, string _statename){
		return _animator.GetCurrentAnimatorStateInfo (0).IsName (_statename);
	}

	public static void SetParentKeepTransform( Transform _child, Transform _parent){
		Vector3 pos = _child.localPosition;
		Vector3 scale = _child.localScale;
		Quaternion q = _child.localRotation;

		_child.SetParent (_parent);

		_child.localPosition = pos;
		_child.localScale = scale;
		_child.localRotation = q ;
	}

	public static bool IsValidString(string _str){
		return _str != null && _str != "";
	}

    /// <summary>
    /// Returns a SIGNED angle between the two vector ( counterclockwise )
    /// </summary>
    public static float AngleBetweenVectors(Vector3 v1, Vector3 v2)
    {
        return Mathf.DeltaAngle(Mathf.Atan2(v1.y, v1.x) * Mathf.Rad2Deg,
                                Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg);
    }    

    /// <summary>
    /// Copies the properties to a destination object.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="destination">The destination.</param>
    public static void CopyProperties(object source, object destination)
    {
        // If any this null throw an exception
        if (source == null || destination == null)
            throw new System.Exception("Source or/and Destination Objects are null");
        // Getting the Types of the objects
        Type typeDest = destination.GetType();
        Type typeSrc = source.GetType();

        // Iterate the Properties of the source instance and  
        // populate them from their desination counterparts  
        PropertyInfo[] srcProps = typeSrc.GetProperties();
        foreach (PropertyInfo srcProp in srcProps)
        {
            if (!srcProp.CanRead)
            {
                continue;
            }
            PropertyInfo targetProperty = typeDest.GetProperty(srcProp.Name);
            if (targetProperty == null)
            {
                continue;
            }
            if (!targetProperty.CanWrite)
            {
                continue;
            }
            if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
            {
                continue;
            }
            if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
            {
                continue;
            }
            if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
            {
                continue;
            }
            // Passed all tests, lets set the value
            targetProperty.SetValue(destination, srcProp.GetValue(source, null), null);
        }
    }

    /// <summary>
    /// Returns the number of elements inside an enum. 
    /// Usage : EnumCount( MyEnum.AnyValue )
    /// </summary>
    public static int EnumCount( Enum _enum ){        
        return Enum.GetNames(_enum.GetType()).Length;
    }
}
