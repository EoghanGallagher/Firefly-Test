using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Galvanic.PipPlugin;


public class PipDataFilter
{
	LinkedList<float> m_dataBuffer;
	LinkedList<float> m_dataTimes;

	private float m_currentValue = 0;
	private float m_currentTime = 0;
	private float m_lastBufferUpdateTime = 0f;

	//Config Values
	private float m_stressWeight = -1f;
	private float m_relaxWeight = 1f;
	private float m_steadyWeight = 0.25f;

	private float m_valueUpdateIncrement = 1f;

	private float m_valueMin = -10f;
	private float m_valueMax = 10f;

	private float m_avgWindowTime = 4f;

	private float m_flatlineFactor = 1f;

	private float m_bufferUpdateInterval = 0.125f;

	public bool Paused { get; set; }


	public PipDataFilter()
	{
		Paused = false;
		m_dataBuffer = new LinkedList<float>();
		m_dataTimes = new LinkedList<float>();
	}

	public void ConfigureFilter(float valueMin, float valueMax,float avgWindowTime,float stressWeight,
		float relaxWeight,float steadyWeight,float flatlineFactor,float valueUpdateIncrement)
	{
		m_valueMin = valueMin;
		m_valueMax = valueMax;
		m_avgWindowTime = avgWindowTime;
		m_stressWeight = stressWeight;
		m_relaxWeight = relaxWeight;
		m_steadyWeight = steadyWeight;
		m_flatlineFactor = flatlineFactor;
		m_valueUpdateIncrement = valueUpdateIncrement;
	}
		
	public void reset()
	{
		m_dataBuffer.Clear();
		m_dataTimes.Clear();
	}
	public float getCurrentValue()
	{



		float currentFilteredVal = 0;

		foreach(float dataPoint in m_dataBuffer)
        {
			currentFilteredVal += dataPoint;
		}

		currentFilteredVal /= m_dataBuffer.Count;
		currentFilteredVal = (((currentFilteredVal - m_valueMin) / (m_valueMax - m_valueMin))*2)-1;

		currentFilteredVal = currentFilteredVal < -1 ? -1 : currentFilteredVal;
		currentFilteredVal = currentFilteredVal > 1 ? 1 : currentFilteredVal;

		return currentFilteredVal;
	}

	public void update(float dt)
	{

		Debug.Log ( "Update Called" );

		if(!Paused)
		{
			m_currentTime += dt;

			if (m_currentTime - m_lastBufferUpdateTime > m_bufferUpdateInterval) {
				m_currentValue *= m_flatlineFactor;
				m_dataBuffer.AddFirst(m_currentValue);
				m_dataTimes.AddFirst(m_currentTime);

				while ((m_currentTime - m_dataTimes.Last.Value) > m_avgWindowTime ) {
					m_dataBuffer.RemoveLast();
					m_dataTimes.RemoveLast();
				}
			}
		}
	}
	#if !EMULATED_PIP
	public void setEvent( PipStressTrend stressTrend )
	{

		switch ( stressTrend )
		{
		case PipStressTrend.Relaxing:
			
					m_currentValue += ( m_valueUpdateIncrement * m_relaxWeight );

				break;
		
		case PipStressTrend.Stressing:
			
					m_currentValue += ( m_valueUpdateIncrement * m_stressWeight );
			
				break;
		
		case PipStressTrend.Steady:
			
					m_currentValue += ( m_valueUpdateIncrement * m_steadyWeight );
			
				break;
		}

		m_currentValue = m_currentValue < m_valueMin ? m_valueMin : m_currentValue;
		m_currentValue = m_currentValue > m_valueMax ? m_valueMax : m_currentValue;

	}
	#else
	public void setEvent(PipEmulatorEvent stressTrend)
	{

		switch (stressTrend)
		{
		case PipEmulatorEvent.Relax:
			m_currentValue += (m_valueUpdateIncrement*m_relaxWeight);
			break;
		case PipEmulatorEvent.Stress:
			m_currentValue += (m_valueUpdateIncrement*m_stressWeight);
			break;
		case PipEmulatorEvent.Steady:
			m_currentValue += (m_valueUpdateIncrement*m_steadyWeight);
			break;
		case PipEmulatorEvent.None:
			break;
		}

		m_currentValue = m_currentValue < m_valueMin ? m_valueMin : m_currentValue;
		m_currentValue = m_currentValue > m_valueMax ? m_valueMax : m_currentValue;

		//Debug.Log ( stressTrend + " " + m_currentValue );
	}
	#endif
}
