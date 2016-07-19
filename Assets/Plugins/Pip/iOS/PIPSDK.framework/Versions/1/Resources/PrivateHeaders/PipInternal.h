/*
 * PipInternal.h
 *
 * Copyright (c) 2014 Galvanic Ltd.
 * All rights reserved.
 *
 * This software is the confidential and proprietary information of Galvanic Limited.
 */

#import <Foundation/Foundation.h>

#if TARGET_OS_IPHONE
#import <CoreBluetooth/CoreBluetooth.h>
#else
#import <IOBluetooth/IOBluetooth.h>
#endif

/*
 Extension of the Pip class to keep certain properties and methods visible only 
 internally to the framework.
 */
@interface Pip() <CBPeripheralDelegate>

@property (nonatomic, strong) NSTimer* connectionTimer;
@property (nonatomic, retain) CBPeripheral *peripheral;

- (id)initWithPeripheral:(CBPeripheral *)peripheral;
- (void)setDiscoveryIndex: (NSInteger)index;
- (void)setRegisteredIndex: (NSInteger)index;
- (void) peripheralConnected;
- (void) peripheralDisconnected;
- (void) peripheralConnectionFailed: (NSUInteger) status;
- (void) setSuspended;
- (void) setResumed;

#ifdef _PIPSDK_TEST_ANALYZER_
-(double) testAnalyzerUpdate:(int)sample;
-(void) resetAnalyzer;
#endif

@end
