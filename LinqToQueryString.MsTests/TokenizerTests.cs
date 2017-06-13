﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace LinqToQuerystring.MsTests
{
  [TestClass]
  public class TokenizerTests
  {
    private void VerifySequence(string value, params TokenType[] types)
    {
      var actual = OData.Tokenize(value).Select(t => t.Type).ToArray();
      CollectionAssert.AreEqual(types, actual);
    }

    [TestMethod]
    public void Tokens_SimpleUrl()
    {
      VerifySequence("http://host/service/Products"
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier);
    }

    [TestMethod]
    public void Tokens_FunctionNoParam()
    {
      VerifySequence("http://host/service/Products/Model.MostExpensive()"
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.OpenParen, TokenType.CloseParen);
    }

    [TestMethod]
    public void Tokens_FunctionOneParam()
    {
      VerifySequence("http://host/service/ProductsByCategoryId(categoryId=2)"
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.OpenParen, TokenType.Identifier, TokenType.Equals, TokenType.Integer, TokenType.CloseParen);
    }

    [TestMethod]
    public void Tokens_FunctionTwoParams()
    {
      VerifySequence("https://host/service/Orders(1)/Items(OrderID=1,ItemNo=2)"
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.OpenParen, TokenType.Integer, TokenType.CloseParen
        , TokenType.PathSeparator, TokenType.Identifier
        , TokenType.OpenParen, TokenType.Identifier, TokenType.Equals, TokenType.Integer, TokenType.Comma
        , TokenType.Identifier, TokenType.Equals, TokenType.Integer, TokenType.CloseParen);
    }

    [TestMethod]
    public void Tokens_FunctionWithAlias()
    {
      VerifySequence("http://host/service/ProductsByColor(color=@color)?@color='red'"
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.OpenParen, TokenType.Identifier, TokenType.Equals, TokenType.Alias, TokenType.CloseParen
        , TokenType.Question, TokenType.Alias, TokenType.Equals, TokenType.String);
    }

    [TestMethod]
    public void Tokens_SingleItem()
    {
      VerifySequence("http://host/service/Categories(1)"
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.OpenParen, TokenType.Integer, TokenType.CloseParen);
    }

    [TestMethod]
    public void Tokens_SingleItemPath()
    {
      VerifySequence("http://host/service/Products(1)/Supplier"
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.OpenParen, TokenType.Integer, TokenType.CloseParen
        , TokenType.PathSeparator, TokenType.Identifier);
    }

    [TestMethod]
    public void Tokens_SingleItemFunction()
    {
      VerifySequence("http://host/service/Products(1)/Model.MostRecentOrder()"
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.OpenParen, TokenType.Integer, TokenType.CloseParen
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.OpenParen, TokenType.CloseParen);
    }

    [TestMethod]
    public void Tokens_TwoFunctions()
    {
      VerifySequence("http://host/service/Categories(ID=1)/Products(ID=1)"
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.OpenParen, TokenType.Identifier, TokenType.Equals, TokenType.Integer, TokenType.CloseParen
        , TokenType.PathSeparator, TokenType.Identifier
        , TokenType.OpenParen, TokenType.Identifier, TokenType.Equals, TokenType.Integer, TokenType.CloseParen);
    }

    [TestMethod]
    public void Tokens_EscapeString01()
    {
      var url = "http://host/service/People('O''Neil')";
      VerifySequence(url
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.OpenParen, TokenType.String, TokenType.CloseParen);
      var parts = OData.Tokenize(url).ToArray();
      Assert.AreEqual("O'Neil", parts[8].AsPrimitive());
    }

    [TestMethod]
    public void Tokens_EscapeString02()
    {
      var url = "http://host/service/People(%27O%27%27Neil%27)";
      VerifySequence(url
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.OpenParen, TokenType.String, TokenType.CloseParen);
      var parts = OData.Tokenize(url).ToArray();
      Assert.AreEqual("O'Neil", parts[8].AsPrimitive());
    }

    [TestMethod]
    public void Tokens_EscapeString03()
    {
      var url = "http://host/service/People%28%27O%27%27Neil%27%29";
      VerifySequence(url
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.OpenParen, TokenType.String, TokenType.CloseParen);
      var parts = OData.Tokenize(url).ToArray();
      Assert.AreEqual("O'Neil", parts[8].AsPrimitive());
    }

    [TestMethod]
    public void Tokens_EscapeString04()
    {
      var url = "http://host/service/Categories('Smartphone%2FTablet')";
      VerifySequence(url
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.OpenParen, TokenType.String, TokenType.CloseParen);
      var parts = OData.Tokenize(url).ToArray();
      Assert.AreEqual("Smartphone/Tablet", parts[8].AsPrimitive());
    }

    [TestMethod]
    public void Tokens_FilterQuery01()
    {
      VerifySequence("http://host/service/Categories?$filter=Products/$count gt 0"
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.Question, TokenType.QueryName, TokenType.Equals, TokenType.Identifier
        , TokenType.Navigation, TokenType.Identifier
        , TokenType.Whitespace, TokenType.Operator, TokenType.Whitespace, TokenType.Integer);
    }

    [TestMethod]
    public void Tokens_FilterQuery02()
    {
      VerifySequence("http://host/service/$all/Model.Customer?$filter=contains(Name,'red')"
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.PathSeparator, TokenType.Identifier
        , TokenType.Question, TokenType.QueryName, TokenType.Equals, TokenType.Identifier
        , TokenType.OpenParen, TokenType.Identifier, TokenType.Comma, TokenType.String, TokenType.CloseParen);
    }

    [TestMethod]
    public void Tokens_FilterQuery_BoolLogic()
    {
      var urls = new string[]
      {
        "http://host/service/Products?$filter=Name eq 'Milk' and Price lt 2.55",
        "http://host/service/Products?$filter=Name eq 'Milk' or Price lt 2.55",
      };

      foreach (var url in urls)
      {
        VerifySequence(url
          , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
          , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
          , TokenType.Question, TokenType.QueryName, TokenType.Equals, TokenType.Identifier
          , TokenType.Whitespace, TokenType.Operator, TokenType.Whitespace, TokenType.String
          , TokenType.Whitespace, TokenType.Operator, TokenType.Whitespace, TokenType.Identifier
          , TokenType.Whitespace, TokenType.Operator, TokenType.Whitespace, TokenType.Double);
      }
    }

    [TestMethod]
    public void Tokens_FilterQuery_Operators()
    {
      var urls = new string[]
      {
        "http://host/service/Products?$filter=Name eq 'Milk'",
        "http://host/service/Products?$filter=Name ne 'Milk'",
        "http://host/service/Products?$filter=Name gt 'Milk'",
        "http://host/service/Products?$filter=Name ge 'Milk'",
        "http://host/service/Products?$filter=Name lt 'Milk'",
        "http://host/service/Products?$filter=Name le 'Milk'",
      };

      foreach (var url in urls)
      {
        VerifySequence(url
          , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
          , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
          , TokenType.Question, TokenType.QueryName, TokenType.Equals, TokenType.Identifier
          , TokenType.Whitespace, TokenType.Operator, TokenType.Whitespace, TokenType.String);
      }
    }


    [TestMethod]
    public void Tokens_FilterQuery_DataTypes()
    {
      var urls = new Dictionary<string, TokenType>()
      {
        { "?$filter=NullValue eq null", TokenType.Null },
        { "?$filter=TrueValue eq true", TokenType.True },
        { "?$filter=FalseValue eq false", TokenType.False },
        { "?$filter=BinaryValue eq binary'T0RhdGE'", TokenType.Base64 },
        { "?$filter=BinaryValue eq X'ffa3cd'", TokenType.Binary },
        { "?$filter=IntegerValue lt -128", TokenType.Integer },
        { "?$filter=IntegerValue lt -128L", TokenType.Long },
        { "?$filter=DoubleValue ge 0.31415926535897931e1", TokenType.Double },
        { "?$filter=DoubleValue ge 0.31415926535897931M", TokenType.Decimal },
        { "?$filter=DoubleValue ge 0.31415926535897931d", TokenType.Double },
        { "?$filter=DoubleValue ge 0.314f", TokenType.Single },
        { "?$filter=SingleValue eq INF", TokenType.PosInfinity },
        { "?$filter=DecimalValue eq 34.95", TokenType.Double },
        { "?$filter=StringValue eq 'Say Hello,then go'", TokenType.String },
        { "?$filter=DateValue eq 2012-12-03", TokenType.Date },
        { "?$filter=DateValue eq datetime'2012-12-03'", TokenType.Date },
        { "?$filter=DateTimeOffsetValue eq 2012-12-03T07:16:23Z", TokenType.Date },
        { "?$filter=DateTimeOffsetValue eq datetimeoffset'2012-12-03T07:16:23Z'", TokenType.Date },
        { "?$filter=DurationValue eq duration'P12DT23H59M59.999999999999S'", TokenType.Duration },
        { "?$filter=DurationValue eq time'P12DT23H59M59.999999999999S'", TokenType.Duration },
        { "?$filter=TimeOfDayValue eq 07:59:59.999", TokenType.TimeOfDay },
        { "?$filter=GuidValue eq 01234567-89ab-cdef-0123-456789abcdef", TokenType.Guid },
        { "?$filter=GuidValue eq guid'01234567-89ab-cdef-0123-456789abcdef'", TokenType.Guid },
        { "?$filter=Int64Value eq 0", TokenType.Integer },
      };
      
      foreach (var url in urls)
      {
        Assert.AreEqual(url.Value, OData.Tokenize(url.Key).Last().Type);
      }
    }

    [TestMethod]
    public void Tokens_OrderBy01()
    {
      VerifySequence("http://host/service/Categories?$orderby=Products/$count"
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.Question, TokenType.QueryName, TokenType.Equals, TokenType.Identifier
        , TokenType.Navigation, TokenType.Identifier);
    }

    [TestMethod]
    public void Tokens_Expand01()
    {
      VerifySequence("http://host/service/Orders?$expand=Customer/Model.VipCustomer"
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.Question, TokenType.QueryName, TokenType.Equals, TokenType.Identifier
        , TokenType.Navigation, TokenType.Identifier, TokenType.Period, TokenType.Identifier);
    }

    [TestMethod]
    public void Tokens_Search01()
    {
      VerifySequence("http://host/service/$all?$search=red"
        , TokenType.Scheme, TokenType.PathSeparator, TokenType.Authority
        , TokenType.PathSeparator, TokenType.Identifier, TokenType.PathSeparator, TokenType.Identifier
        , TokenType.Question, TokenType.QueryName, TokenType.Equals, TokenType.Identifier);
    }

    [TestMethod]
    public void Tokens_Query()
    {
      VerifySequence("?$format=json&$filter=Name eq 'Apple'"
        , TokenType.Question, TokenType.QueryName, TokenType.Equals, TokenType.Identifier
        , TokenType.Amperstand, TokenType.QueryName, TokenType.Equals, TokenType.Identifier
        , TokenType.Whitespace , TokenType.Operator, TokenType.Whitespace, TokenType.String);
    }
  }
}