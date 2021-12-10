using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
public class MyBinaryHeap<T> : ICollection<T> where T : IComparable<T>
{
	private const int m_inDefaultSize = 32;
	// Parameters
    private T[] m_aData = null;
    private int m_iCount = 0;
    private int m_iMaxSize = 0;
    private bool m_bSorted = false;
	
	// Properties
	public bool IsReadOnly
    {
        get { return false; }
    }
    public int Count
    {
        get { return m_iCount; }
    }
    public int MaxSize
    {
        get { return m_iMaxSize; }
    }
	
	public MyBinaryHeap()
    {
		m_iMaxSize = m_inDefaultSize;
		m_aData = new T[m_iMaxSize];
		m_bSorted = false;
		m_iCount = 0;
    }
	
	~MyBinaryHeap()
	{
		m_aData = null;
	}
	
	private void ResizeHeap()
	{
		int iNewSize = m_iMaxSize + m_inDefaultSize;
		m_iMaxSize = iNewSize;
		T[] temp = new T[iNewSize];
        Array.Copy(m_aData, temp, m_iCount);
        m_aData = temp;
	}
	
	private int Parent(int index)
    {
		int iPIndex = (index - 1) >> 1;
        return iPIndex;
    }
    private int Child1(int index)
    {
		int iChild = index*2 + 1;
        return iChild;
    }
    private int Child2(int index)
    {
        int iChild = index*2 + 2;
        return iChild;
    }
	
	private void UpHeap(int iStartIndex)
    {
        int isIndex= iStartIndex;
        T item = m_aData[isIndex];
        int pIndex = Parent(isIndex);
        while (pIndex > -1 && item.CompareTo(m_aData[pIndex]) < 0)
        {
            m_aData[isIndex] = m_aData[pIndex];
            isIndex = pIndex;
			
            pIndex = Parent(isIndex);
        }
        m_aData[isIndex] = item;
		m_bSorted = false;
    }
	
	private void DownHeap()
    {
        int nIndex = 0;
        int sIndex = 0;
		int ch1, ch2;
        T item = m_aData[sIndex];
        while(true) {
            ch1 = Child1(sIndex);
            if (ch1 >= m_iCount) break;
            ch2 = Child2(sIndex);
            if (ch2 >= m_iCount) {
                nIndex = ch1;
            } else {
                nIndex = m_aData[ch1].CompareTo(m_aData[ch2]) < 0 ? ch1 : ch2;
            }
            if (item.CompareTo(m_aData[nIndex]) > 0) {
                m_aData[sIndex] = m_aData[nIndex]; //Swap nodes
                sIndex = nIndex;
            } else {
                break;
            }
        }
        m_aData[sIndex] = item;
		m_bSorted = false;
    }
	
	private void EnsureSort()
    {
        if (m_bSorted) return;
        Array.Sort(m_aData, 0, m_iCount);
        m_bSorted = true;
    }
	
	public void Clear()
    {
        m_iCount = 0;
        m_aData = new T[m_iMaxSize];
    }
	
    public void Add(T item)
    {
        if (m_iCount >= m_iMaxSize) {
			ResizeHeap();
        }
        m_aData[m_iCount] = item;
        UpHeap(m_iCount);
        m_iCount++;
    }
	
	public T PopRoot()
	{
		if(m_iCount == 0) {
			return default(T);
        }
		
        T root = m_aData[0];
        m_iCount--;
		
		// Move last node to top.
        m_aData[0] = m_aData[m_iCount];
        m_aData[m_iCount] = default(T);
        DownHeap();
        return root;
	}
	
	public bool Remove(T item)
    {
        EnsureSort();
        int i = Array.BinarySearch<T>(m_aData, 0, m_iCount, item);
        if (i < 0) return false; // Not found
        Array.Copy(m_aData, i + 1, m_aData, i, m_iCount - i);
        m_aData[m_iCount] = default(T);
        m_iCount--;
        return true;
    }
	
    public bool Contains(T item)
    {
        EnsureSort();
		bool bFound = true;
		if(Array.BinarySearch<T>(m_aData, 0, m_iCount, item) < 0) {
			bFound = false;
		}
        return bFound;
    }
	
    public void CopyTo(T[] array, int arrayIndex)
    {
        EnsureSort();
        Array.Copy(m_aData, array, m_iCount);
    }
	
	public IEnumerator<T> GetEnumerator()
    {
        EnsureSort();
        for (int i = 0; i < m_iCount; i++) {
            yield return m_aData[i];
        }
    }
	
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
