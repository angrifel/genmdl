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
  using ModelGenerator.CSharp.Services;
  using ModelGenerator.Model;
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using Xunit;

  public class CSharpGeneratorTests
  {
    [Fact]
    public void TestGenerateOutputOnEmptySpec()
    {
      // arrange
      var spec = 
        new Spec
        {
          Targets = new Dictionary<string, TargetInfo>
          {
            { Constants.CSharpTarget, new TargetInfo { Path = "some_path", Namespace = "Blogged"} }
          }
        };
      var generator = new CSharpGenerator();

      // act
      var outputs = generator.GenerateOutputs(spec);

      // assert
      Assert.Equal(0, outputs.Count());
    }

    [Fact]
    public void TestGenerateOutputWithEnum()
    {
      // arrange
      var spec =
        new Spec
        {
          Enums = new Dictionary<string, List<EnumMember>>
          {
            {
              "blog_entry_status",
              new List<EnumMember> { new EnumMember { Name = "draft" }, new EnumMember { Name = "final" } }
            }
          },
          Targets = new Dictionary<string, TargetInfo>
          {
            { Constants.CSharpTarget, new TargetInfo { Path = "some_path", Namespace = "BlogModel"} }
          }
        };

      var generator = new CSharpGenerator();

      // act
      var outputs = generator.GenerateOutputs(spec);

      // assert
      var outputList = outputs?.ToList();
      Assert.NotNull(outputs);
      Assert.Equal(1, outputList.Count);
      Assert.True(outputList.Exists(_ => 
          _.Path == Path.Combine("some_path", "BlogEntryStatus.cs") &&
          ((CSharpNamespace)_.GenerationRoot).Name == "BlogModel" &&
          ((CSharpNamespace)_.GenerationRoot).Types.Count == 1 &&
          ((CSharpEnum)((CSharpNamespace)_.GenerationRoot).Types[0]).Name == "BlogEntryStatus" &&
          ((CSharpEnum)((CSharpNamespace)_.GenerationRoot).Types[0]).Members.Count == 2 &&
          ((CSharpEnum)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].Name == "Draft" &&
          ((CSharpEnum)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].Value == null &&
          ((CSharpEnum)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].Name == "Final" &&
          ((CSharpEnum)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].Value == null
        ));
    }

    [Fact]
    public void TestGenerateOutputWithClass ()
    {
      // arrange
      var spec =
        new Spec
        {
          Entities = new Dictionary<string, EntityInfo>
          {
            {
              "blog_entry",
              new EntityInfo
              {
                Members =
                new OrderedDictionary<string, IEntityMemberInfo>
                {
                  { "id", new EntityMemberInfo { Type = "int" } },
                  { "title", new EntityMemberInfo { Type = "string"} }
                }
              }
            }
          },
          Targets = new Dictionary<string, TargetInfo>
          {
            { Constants.CSharpTarget, new TargetInfo { Path = "some_path", Namespace = "BlogModel"} }
          }
        };
      var ammendment = new AmmendmentFactory().CreateAmmendment(Constants.CSharpTarget);
      ammendment.AmmedSpecification(spec);
      var generator = new CSharpGenerator();

      // act
      var outputs = generator.GenerateOutputs(spec);

      // assert
      var outputList = outputs?.ToList();
      Assert.NotNull(outputs);
      Assert.Equal(1, outputList.Count);
      Assert.True(outputList.Exists(_ =>
          _.Path == Path.Combine("some_path", "BlogEntry.cs") &&
          ((CSharpNamespace)_.GenerationRoot).Name == "BlogModel" &&
          ((CSharpNamespace)_.GenerationRoot).Types.Count == 1 &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Name == "BlogEntry" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members.Count == 2 &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].Name == "Id" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].Type == "int" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.NoRequiredAttribute &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].Name == "Title" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].Type == "string" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.IssueRequiredAllowEmptyStrings
        ));
    }

    [Fact]
    public void TestModelWithMultipleEntities()
    {
      // arrange
      var spec = new Spec
      {
        Targets = new Dictionary<string, TargetInfo>
        {
          {
            Constants.CSharpTarget,
            new TargetInfo
            {
              Namespace = "Blog.Model.Data",
              Path = "Blog.Model\\Data",
              TypeAliases = new Dictionary<string, string>
              {
                { "id_t", "int" }
              }
            }
          }
        },
        Enums = new Dictionary<string, List<EnumMember>>
        {
          {
            "blog_post_status",
            new List<EnumMember>
            {
              new EnumMember { Name = "draft" },
              new EnumMember { Name = "final" }
            }
          } 
        },
        Entities = new Dictionary<string, EntityInfo>
        {
          {
            "author",
            new EntityInfo
            {
              Members =
                new OrderedDictionary<string, IEntityMemberInfo>
                {
                  { "id", new EntityMemberInfo { Type = "id_t" } },
                  { "name", new EntityMemberInfo { Type = "string" } },
                  { "alias", new EntityMemberInfo { Type = "string" } }
                }
            }
          },
          {
            "blog",
            new EntityInfo
            {
              Members =
                new OrderedDictionary<string, IEntityMemberInfo>
                {
                  { "id", new EntityMemberInfo { Type = "id_t" } },
                  { "title", new EntityMemberInfo { Type = "string" } },
                  { "posts", new EntityMemberInfo { Type = "blog_post", IsCollection = true } },
                  { "author", new EntityMemberInfo { Type = "author" } }
                }
            }
          },
          {
            "blog_post",
            new EntityInfo
            {
              Members =
                new OrderedDictionary<string, IEntityMemberInfo>
                {
                  { "id", new EntityMemberInfo { Type = "id_t" } },
                  { "date_published", new EntityMemberInfo { Type = "datetime", IsNullable = true } },
                  { "description", new EntityMemberInfo { Type = "string", IsNullable = true } },
                  { "comments", new EntityMemberInfo { Type = "comment", IsCollection = true } },
                  { "status", new EntityMemberInfo { Type = "blog_post_status" } }
                }
            }
          },
          {
            "comment",
            new EntityInfo
            {
              Members =
                new OrderedDictionary<string, IEntityMemberInfo>
                {
                  { "id", new EntityMemberInfo { Type = "id_t" } },
                  { "text", new EntityMemberInfo { Type = "string" } },
                  { "shared_in_fb", new EntityMemberInfo { Type = "bool", Exclude = { "typescript" } } }
                }
            }
          }
        }
      };

      var ammendment = new AmmendmentFactory().CreateAmmendment(Constants.CSharpTarget);
      ammendment.AmmedSpecification(spec);
      var generator = new CSharpGenerator();

      // act
      var outputs = generator.GenerateOutputs(spec);

      // assert
      var outputList = outputs?.ToList();
      Assert.NotNull(outputs);

      Assert.Equal(5, outputList.Count);
      Assert.True(outputList.Exists(_ =>
          _.Path == Path.Combine("Blog.Model", "Data", "BlogPostStatus.cs") &&
          ((CSharpNamespace)_.GenerationRoot).Name == "Blog.Model.Data" &&
          ((CSharpNamespace)_.GenerationRoot).Types.Count == 1 &&
          ((CSharpEnum)((CSharpNamespace)_.GenerationRoot).Types[0]).Name == "BlogPostStatus" &&
          ((CSharpEnum)((CSharpNamespace)_.GenerationRoot).Types[0]).Members.Count == 2 &&
          ((CSharpEnum)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].Name == "Draft" &&
          ((CSharpEnum)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].Value == null &&
          ((CSharpEnum)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].Name == "Final" &&
          ((CSharpEnum)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].Value == null
        ));

      Assert.True(outputList.Exists(_ =>
          _.Path == Path.Combine("Blog.Model", "Data", "Author.cs") &&
          ((CSharpNamespace)_.GenerationRoot).Name == "Blog.Model.Data" &&
          ((CSharpNamespace)_.GenerationRoot).Types.Count == 1 &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Name == "Author" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members.Count == 3 &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].Name == "Id" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].Type == "int" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.NoRequiredAttribute &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].Name == "Name" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].Type == "string" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.IssueRequiredAllowEmptyStrings &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[2].Name == "Alias" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[2].Type == "string" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[2].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.IssueRequiredAllowEmptyStrings
          ));

      Assert.True(outputList.Exists(_ =>
          _.Path == Path.Combine("Blog.Model", "Data", "Blog.cs") &&
          ((CSharpNamespace)_.GenerationRoot).Name == "Blog.Model.Data" &&
          ((CSharpNamespace)_.GenerationRoot).Types.Count == 1 &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Name == "Blog" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members.Count == 4 &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].Name == "Id" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].Type == "int" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.NoRequiredAttribute &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].Name == "Title" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].Type == "string" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.IssueRequiredAllowEmptyStrings &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[2].Name == "Posts" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[2].Type == "IList<BlogPost>" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[2].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.IssueRequired &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[3].Name == "Author" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[3].Type == "Author" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[3].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.IssueRequired
          ));

      Assert.True(outputList.Exists(_ =>
          _.Path == Path.Combine("Blog.Model", "Data", "BlogPost.cs") &&
          ((CSharpNamespace)_.GenerationRoot).Name == "Blog.Model.Data" &&
          ((CSharpNamespace)_.GenerationRoot).Types.Count == 1 &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Name == "BlogPost" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members.Count == 5 &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].Name == "Id" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].Type == "int" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.NoRequiredAttribute &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].Name == "DatePublished" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].Type == "DateTimeOffset?" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.NoRequiredAttribute &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[2].Name == "Description" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[2].Type == "string" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[2].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.NoRequiredAttribute &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[3].Name == "Comments" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[3].Type == "IList<Comment>" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[3].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.IssueRequired &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[4].Name == "Status" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[4].Type == "BlogPostStatus" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[4].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.NoRequiredAttribute
          ));

      Assert.True(outputList.Exists(_ =>
          _.Path == Path.Combine("Blog.Model", "Data", "Comment.cs") &&
          ((CSharpNamespace)_.GenerationRoot).Name == "Blog.Model.Data" &&
          ((CSharpNamespace)_.GenerationRoot).Types.Count == 1 &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Name == "Comment" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members.Count == 3 &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].Name == "Id" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].Type == "int" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[0].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.NoRequiredAttribute &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].Name == "Text" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].Type == "string" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[1].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.IssueRequiredAllowEmptyStrings &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[2].Name == "SharedInFb" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[2].Type == "bool" &&
          ((CSharpClass)((CSharpNamespace)_.GenerationRoot).Types[0]).Members[2].RequiredAttributeBehavior == CSharpRequiredAttributeBehavior.NoRequiredAttribute
          ));
    }
  }
}
