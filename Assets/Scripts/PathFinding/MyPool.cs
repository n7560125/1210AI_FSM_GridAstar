using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPool<T> : IEnumerable<T>
{
	public struct PoolElement
	{
		public int m_iElementIndex;
		public string m_sElementName;
		public bool m_bActive;
		public T m_Item;
	};
	private int m_iDefaultSize;
	private int m_iMaxSize;
	private bool m_bResize;
	private PoolElement[] m_ElementPool;
	
	public PoolElement[] ElementPool
	{
		get { return m_ElementPool; }
	}
	
	public MyPool()
    {
		m_iDefaultSize = 0;
        m_ElementPool = null;
    	m_bResize = false;
    }
	
	private int GetUnActiveElement()
	{
		int i = 0;
		for(i = 0; i < m_iMaxSize; i++) {
			if(m_ElementPool[i].m_bActive == false) {
				break;
			}
		}	
		if(i == m_iMaxSize) {
			if(m_bResize) {
				int iNewSize = m_iMaxSize + m_iDefaultSize;
				PoolElement [] temp = new PoolElement[iNewSize];
				Array.Copy(m_ElementPool, temp, m_iMaxSize);
				m_ElementPool = temp;
				return m_iMaxSize;
			} else {
				return -1;	
			}
		} else {
			return i;
		}
	}
	
	private int FindElementIndexByIndex(int index)
	{
		int i = 0; 
		for(i = 0; i < m_iMaxSize; i++) {
			if(m_ElementPool[i].m_iElementIndex == index) {
				break;
			}
		}
		
		if(i < m_iMaxSize) {
			return i;	
		} else {
			return -1;
		}
	}
	
	private int FindElementIndex(string sName)
	{
		int i = 0; 
		for(i = 0; i < m_iMaxSize; i++) {
			if(m_ElementPool[i].m_sElementName == sName) {
				break;
			}
		}
		
		if(i < m_iMaxSize) {
			return i;	
		} else {
			return -1;
		}
	}
	
	public void Init(int iDefaultSize, bool bCanResize)
	{
		if(iDefaultSize == 0) {
			iDefaultSize = 32;
		}
		m_iDefaultSize = iDefaultSize;
		m_iMaxSize = m_iDefaultSize;
		m_bResize = bCanResize;
		m_ElementPool = new PoolElement[m_iMaxSize];
		int i = 0;
		for(i = 0; i < m_iMaxSize; i++) {
			m_ElementPool[i].m_iElementIndex = -1;
			m_ElementPool[i].m_sElementName = "Empty";
			m_ElementPool[i].m_bActive = false;
			m_ElementPool[i].m_Item = default(T);
		}
	}
	
	public void AddElementToPool(T item, string sName, int itemInnerIndex)
	{
		if(FindElementIndex(sName) >= 0) {
			// Already in pool.
			return;	
		}
		
		int index = GetUnActiveElement();
		if(index < 0) {
			return;	
		}
		
		m_ElementPool[index].m_iElementIndex = itemInnerIndex;
		m_ElementPool[index].m_sElementName = sName;
		m_ElementPool[index].m_bActive = true;
		m_ElementPool[index].m_Item = item;
	}
	
	public void AddElementToPool(T item, string sName)
	{
		if(FindElementIndex(sName) >= 0) {
			// Already in pool.
			return;	
		}
		
		int index = GetUnActiveElement();
		if(index < 0) {
			return;	
		}
		
		m_ElementPool[index].m_iElementIndex = -1;
		m_ElementPool[index].m_sElementName = sName;
		m_ElementPool[index].m_bActive = true;
		m_ElementPool[index].m_Item = item;
	}
	
	public T GetItemFromPool(int index)
	{
		int id = FindElementIndexByIndex(index);
		if(id < 0) {
			return default(T);	
		}
		return m_ElementPool[id].m_Item;
	}
	
	public T GetItemFromPool(string sName)
	{
		int index = FindElementIndex(sName);
		if(index < 0) {
			return default(T);	
		}
		
		return m_ElementPool[index].m_Item;
	}
	
	
    public IEnumerator<T> GetEnumerator()
    {
        foreach (PoolElement item in m_ElementPool) {
			if(item.m_bActive) {
				yield return item.m_Item;
			}
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

}
