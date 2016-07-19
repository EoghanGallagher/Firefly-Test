/*
 * WindowedAverage.h
 *
 * Copyright (c) 2014 Galvanic Ltd.
 * All rights reserved.
 *
 * This software is the confidential and proprietary information of Galvanic Limited.
 */


#ifndef _WINDOWED_AVERAGE_H
#define _WINDOWED_AVERAGE_H

#import <Foundation/Foundation.h>

/*
 Class that maintains a circular buffer of N samples, and maintains the average
 of the samples currently in the circular buffer.
 */
@interface WindowedAverage : NSObject
@property (readonly) float average;

- (id) initWithWindowLength:(int) windowLength;
- (float) update:(NSInteger) sample;
- (void) reset;

@end

#endif 
