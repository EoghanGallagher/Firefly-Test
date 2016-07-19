/*
 * SlopeCalculator.h
 *
 * Copyright (c) 2014 Galvanic Ltd.
 * All rights reserved.
 *
 * This software is the confidential and proprietary information of Galvanic Limited.
 */

#ifndef _SLOPECALCULATOR_H
#define _SLOPECALCULATOR_H
/*
 * Slope calculation algorithm. Uses regression line formula to estimate slope
 * of a window of samples. Uses a circular buffer to buffer the moving window.
 *
 * The standard least squares formula for estimating the slope of the best
 * fit line to a set of (x,y) data points is given by:
 *
 * slope = Sigma{(x - xmean)*(y - ymean)} / Sigma{(x-xmean)^2}
 *
 * For optimisation, and because we care only about the slope parameter and
 * not the time of occurrence of samples, we assume that our sample buffer
 * is centred at time x=0, i.e. -N/2 <= x <= N/2 (using integer division and the
 * constraint that the sample buffer size N is odd). In that case, the mean value of
 * x is zero, which allows us to precalculate S = Sigma{(x-xmean)^2}.
 *
 * Then, the regression formula simplifies to:
 *
 * slope = Sigma{x*(y - ymean)} / S
 */

#import <Foundation/Foundation.h>

@interface SlopeCalculator : NSObject

@property (readonly) double slope;

- (id) initWithSampleWindowLength:(int)sampleWindowlength;
// Add new data point to sample buffer.
- (double) update:(NSInteger) sample;
// Reset sample buffer
- (void) reset;

@end

#endif
