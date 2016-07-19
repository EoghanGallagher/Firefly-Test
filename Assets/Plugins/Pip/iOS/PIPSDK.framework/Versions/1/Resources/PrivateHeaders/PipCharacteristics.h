/*
 * PipCharacteristics.h
 *
 * Copyright (c) 2014 Galvanic Ltd.
 * All rights reserved.
 *
 * This software is the confidential and proprietary information of Galvanic Limited.
 */

#ifndef _PIP_CHARACTERISTICS_H
#define _PIP_CHARACTERISTICS_H

#import <Foundation/Foundation.h>
#if TARGET_OS_IPHONE
#import <CoreBluetooth/CoreBluetooth.h>
#else
#import <IOBluetooth/IOBluetooth.h>
#endif


typedef NS_ENUM(NSUInteger, PipCharacteristicList) {
    PIP_STREAM_ENABLE,
    PIP_SAMPLE_VALUE,
    PIP_DAC_LEVEL,
    PIP_BATTERY_LEVEL,
    PIP_BATTERY_EVENT,
    PIP_VERSION,
    PIP_NAME,
    PIP_PING,
    PIP_POWER_OFF,
    PIP_POWER_SAVE,
    PIP_RESET,
    PIP_NUM_CHAR
};

typedef NS_ENUM(NSUInteger,LinkStatus_t)
{
	LS_UNKNOWN = 1,
	LS_OK,
	LS_TOO_CLOSE,
	LS_INTERFERENCE,
	LS_TOO_FAR
};

static NSString *const PIP_SERVICE_CHAR=@"43445C00-29EC-4212-B5B3-3AC9A3FE95C6";
static NSString * const PIP_CHAR_NAMES[] = {
    @"PIP_STREAM_ENABLE",
    @"PIP_SAMPLE_VALUE",
    @"PIP_DAC_LEVEL",
    @"PIP_BATTERY_LEVEL",
    @"PIP_BATTERY_EVENT",
    @"PIP_VERSION",
    @"PIP_NAME",
    @"PIP_PING",
    @"PIP_POWER_OFF",
    @"PIP_POWER_SAVE",
    @"PIP_RESET"
};

static NSString * const PIP_CHAR_UUIDS[] = {
    @"A73147A4-4B8D-411E-AC6A-07839E9FF18C",
    @"D6161AEC-E928-4C02-BFAA-9865FEE9143B",
    @"A55BDF88-9FAB-407F-A00E-CDFAF60ED81C",
    @"E6D642E4-F0D5-4B9D-9C4A-075A8EC8E728",
    @"ABAB51E8-315E-4355-96B2-C861F8E9BBC1",
    @"3C661EC1-7D27-4AFD-9BC2-6804CDF72B14",
    @"94CF10C3-9241-45FF-AA8B-C40AE205629F",
    @"5B7234D1-1AB9-4EE1-A407-46B1ADD89885",
    @"3440D338-A892-4869-96C5-B69EC156BB85",
    @"10CBFAFB-7865-4C3F-BA3D-084FD61F5181",
    @"BAC4B688-2AFF-4A00-A503-9200D1406B00"
};

#endif // _PIP_CHARACTERISTICS_H