﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace XmlFormattersWebSite.Models
{
    public class CustomerWithComplexPropertyHavingRequiredOnly
	{
		[Required]
		public int Id { get; set; }

		public string Name { get; set; }

		public Address Address { get; set; }
	}
}