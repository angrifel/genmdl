﻿//  This file is part of gen-mdl - A Source code generator for model definitions.
//  Copyright (c) angrifel

//  Permission is hereby granted, free of charge, to any person obtaining a copy of
//  this software and associated documentation files (the "Software"), to deal in
//  the Software without restriction, including without limitation the rights to
//  use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//  of the Software, and to permit persons to whom the Software is furnished to do
//  so, subject to the following conditions:

//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.

//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.

namespace ModelGenerator.Tests.CSharp
{
  using ModelGenerator.CSharp;
  using System;
  using System.Collections.Generic;
  using System.IO;
  using Xunit;

  public class CSharpClassTests
  {
    [Fact]
    public void TestGenerateClassWithNoMembers()
    {
      // arrange
      var csharpClass = new CSharpClass { Name = "Blog" };
      var output = new StringWriter();
      var expectedOutput =
        "  public class Blog" + Environment.NewLine +
        "  {" + Environment.NewLine +
        "  }" + Environment.NewLine;

      // act
      csharpClass.Generate(output);

      // assert
      var generatedOutput = output.GetStringBuilder().ToString();
      Assert.Equal(expectedOutput, generatedOutput);
    }

    [Fact]
    public void TestGenerateClassWithMembers()
    {
      // arrange
      var idMember = new CSharpClassMember { Name = "Id", Type = "int" };
      var descriptionMembers = new CSharpClassMember { Name = "Description", Type = "string" };
      var csharpClass = new CSharpClass { Name = "Blog", Members = new List<CSharpClassMember> { idMember, descriptionMembers } };
      var output = new StringWriter();
      var expectedOutputWriter = new StringWriter();
      expectedOutputWriter.Write("  public class Blog" + Environment.NewLine);
      expectedOutputWriter.Write("  {" + Environment.NewLine);
      idMember.Generate(expectedOutputWriter);
      expectedOutputWriter.WriteLine();
      descriptionMembers.Generate(expectedOutputWriter);
      expectedOutputWriter.Write("  }" + Environment.NewLine);

      // act
      csharpClass.Generate(output);

      // assert
      var generatedOutput = output.GetStringBuilder().ToString();
      Assert.Equal(expectedOutputWriter.GetStringBuilder().ToString(), generatedOutput);
    }
  }
}
