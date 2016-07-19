/*
 * AnalyzerOutput.h
 *
 * Copyright (c) 2014 Galvanic Ltd.
 * All rights reserved.
 *
 * This software is the confidential and proprietary information of Galvanic Limited.
 */

#ifndef _ANALYZER_OUTPUT_H
#define _ANALYZER_OUTPUT_H

#import <Foundation/Foundation.h>

/**
 * This class encapsulates an output from a signal analyzer. Each output value
 * consists of a descriptive name and a floating point value whose meaning is
 * dependent on the properties of the specific analyzer being used.
 */
@interface AnalyzerOutput : NSObject

/** Human-readable name for this output. */
@property (retain) NSString* label;
/** Current value of this output. */
@property (assign) double value;

- (id) initWithLabel:(NSString*)label andValue:(double)value;

@end

#endif