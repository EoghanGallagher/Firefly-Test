/*
 * SignalAnalyzer.h
 *
 * Copyright (c) 2014 Galvanic Ltd.
 * All rights reserved.
 *
 * This software is the confidential and proprietary information of Galvanic Limited.
 */

#import <Foundation/Foundation.h>
#import "AnalyzerOutput.h"

@interface SignalAnalyzer : NSObject

@property (strong,nonatomic) NSString* analyzerName;
@property (strong,nonatomic) NSMutableArray* outputs;
@property NSInteger numOutputs;

- (id) init;
- (void) updateWithSample:(NSInteger) sample andConductance:(double)conductance;
- (void) reset;

@end
