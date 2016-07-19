/*
 * StandardSignalAnalyzer.h
 *
 * Copyright (c) 2014 Galvanic Ltd.
 * All rights reserved.
 *
 * This software is the confidential and proprietary information of Galvanic Limited.
 */

/**
 * An enumeration for identifying the stress trend outputs of the standard signal analyzer.
 */
typedef NS_ENUM(NSInteger, StandardAnalyzerStressTrend)
{
    /** There was no trend detected for this sample */
    SAStressTrendNone,
    /** A relaxation trend has been detected in the user's electrodermal activity */
    SAStressTrendRelaxing,
    /** A stressing trend has been detected in the user's electrodermal activity */
    SAStressTrendStressing,
    /** A constant trend has been detected in the user's electrodermal activity */
    SAStressTrendConstant
};

/** 
 An enumeration for indexing the outputs of the standard signal analyzer.
 */
typedef NS_ENUM(NSInteger, StandardAnalyzerOutputType)
{
    /** The analyzer's stress trend output */
    SAOutputCurrentTrendEvent = 0,
    /** The analyzer's previous stress trend output */
    SAOutputPrevTrendEvent,
    /** The analyzer's accumulated trend output */
    SAOutputAccumulatedTrend,
#ifdef _PIPSDK_PRO_VERSION_
    SAOutputConductance,
#endif
    /** Sentinel value */
    SAOutputNumOutputs
};
