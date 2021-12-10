using System;
using System.Collections.Generic;

public class PathNode : IComparable<PathNode>
{
	public enum eNodeState
	{
		UnCheck = 0,
		Open = 1,
		Close = 2,
		Block = 3
	};
	private PathNode m_Parent;
	private eNodeState m_eState;
	private int m_iIndex;
	private float m_fFValue;
	private float m_fGValue;
	private float m_fHValue;
	
	public float FValue
	{
		get { return m_fFValue; }
		set { m_fFValue = value; }	
	}
	public float GValue
	{
		get { return m_fGValue; }
		set { m_fGValue = value; }
		
	}
	public float HValue
	{
		get { return m_fHValue; }
		set { m_fHValue = value; }	
		
	}
	public int Index
	{
		get { return m_iIndex; }
		set { m_iIndex = value; }	
		
	}
	public PathNode Parent
	{
		get { return m_Parent; }
		set { m_Parent = value; }	
		
	}
	public eNodeState State
	{
		get { return m_eState; }
		set { m_eState = value; }	
		
	}
	
	public PathNode()
	{
	    m_Parent = null;
		m_eState = eNodeState.UnCheck;
	 	m_iIndex = -1;
	    m_fFValue = float.MaxValue;
		m_fGValue = float.MaxValue;
		m_fHValue = float.MaxValue;
	}
	
	public int CompareTo(PathNode node)
    {
        if (m_fFValue < node.m_fFValue) {
            return -1;
        } else if (m_fFValue > node.m_fFValue) {
            return 1;
        } else {
            return 0;
        }
    }
}
