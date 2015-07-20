﻿using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Elasticsearch.Net;
using Elasticsearch.Net.Connection;
using Elasticsearch.Net.ConnectionPool;
using FluentAssertions;
using Nest;
using Tests.Framework;
using static Tests.Framework.RoundTripper;

namespace Tests.CommonOptions
{
	public class TimeUnits 
	{
		/** #  Time units
		 *Whenever durations need to be specified, eg for a timeout parameter, the duration can be specified 
		 as a whole number representing time in milliseconds, or as a time value like `2d` for 2 days. 
		 * 
		 * ## Using Time units in NEST
		 * NEST uses `TimeUnitExpression` to strongly type this and there are several ways to construct one.
		 *
		 * ### Constructor
		 * The most straight forward way to construct a `TimeUnitExpression` is through its constructor
		 */
		
		[U] public void Constructor()
		{
			var unitString = new TimeUnitExpression("2d");
			var unitComposed = new TimeUnitExpression(2, TimeUnit.Day);
			var unitTimeSpan = new TimeUnitExpression(TimeSpan.FromDays(2));
			var unitMilliseconds = new TimeUnitExpression(1000 * 60 * 60 * 24 * 2);
			
			/**
			* When serializing TimeUnitExpression constructed from a string, composition of factor and interval, or a `TimeSpan`
			* the expression will be serialized as time unit string
			*/
			Expect("2d")
				.WhenSerializing(unitString)
				.WhenSerializing(unitComposed)
				.WhenSerializing(unitTimeSpan);
			/**
			* When constructed from a long representing milliseconds, a long will be serialized
			*/
			Expect(172800000).WhenSerializing(unitMilliseconds);

			/**
			* Milliseconds are always calculated even when not using the constructor that takes a long
			*/

			unitMilliseconds.Milliseconds.Should().Be(1000*60*60*24*2);
			unitComposed.Milliseconds.Should().Be(1000*60*60*24*2);
			unitTimeSpan.Milliseconds.Should().Be(1000*60*60*24*2);
			unitString.Milliseconds.Should().Be(1000*60*60*24*2);
		}
		/**
		* ### Implicit conversion
		* Alternatively `string`, `TimeSpan` and `long` can be implicitly assigned to `TimeUnitExpression` properties and variables 
		*/
		[U]
		[SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
		public void ImplicitConversion()
		{
			TimeUnitExpression oneAndHalfYear = "1.5y";
			TimeUnitExpression twoWeeks = TimeSpan.FromDays(14);
			TimeUnitExpression twoDays = 1000 * 60 * 60 * 24 * 2;
			
			Expect("1.5y").WhenSerializing(oneAndHalfYear);
			Expect("2w").WhenSerializing(twoWeeks);
			Expect(172800000).WhenSerializing(twoDays);

			/**
			* Milliseconds are calculated even when values are not passed as long
			*/
			oneAndHalfYear.Milliseconds.Should().BeGreaterThan(1);
			twoWeeks.Milliseconds.Should().BeGreaterThan(1);

			/**
			* This allows you to do comparisons on the expressions
			*/
			oneAndHalfYear.Should().BeGreaterThan(twoWeeks);
			twoDays.Should().BeLessThan(twoWeeks);
			
			/**
			* And assert equality
			*/
			twoDays.Should().Be(new TimeUnitExpression("2d"));


		}


		
		
	}
}