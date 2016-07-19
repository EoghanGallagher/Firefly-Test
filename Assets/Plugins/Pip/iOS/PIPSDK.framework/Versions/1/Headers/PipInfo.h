/*
 * PipInfo.h
 *
 * Copyright (c) 2014 Galvanic Ltd.
 * All rights reserved.
 *
 * This software is the confidential and proprietary information of Galvanic Limited.
 */

#import <Foundation/Foundation.h>

// PIP unique identifier type.
typedef int PipID;

/**
 Simple class to encapsulate details of a single PIP.
 */
@interface PipInfo : NSObject

/// PIP unique identifier.
@property PipID pipID;

/// PIP Bluetooth "friendly" name.
@property (copy) NSString *name;

/// iOS UDID as a string
@property (copy) NSString *udid;

@end

