﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Assent;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.DotNet.Interactive.Documents.ParserServer;
using Xunit;

namespace Microsoft.DotNet.Interactive.Documents.Tests
{
    public class NotebookParserServerTests_Serialization
    {
        [Fact]
        public void NotebookParseRequest_deserialization_contract()
        {
            var requestJson = GetTestFileContents();
            var request = NotebookParseOrSerializeRequest.FromJson(requestJson);
            using var _ = new AssertionScope();

            request.Type.Should().Be(RequestType.Parse);
            request.Id.Should().Be("the-id");
            request.SerializationType.Should().Be(DocumentSerializationType.Dib);
            request.DefaultLanguage.Should().Be("csharp");
            request
                .Should()
                .BeOfType<NotebookParseRequest>()
                .Which
                .RawData
                .Should()
                .Equal(new byte[] { 0x01, 0x02, 0x03 });
        }

        [Fact]
        public void NotebookSerializeRequest_deserialization_contract()
        {
            var requestJson = GetTestFileContents();
            var request = NotebookParseOrSerializeRequest.FromJson(requestJson);
            using var _ = new AssertionScope();

            request.Type.Should().Be(RequestType.Serialize);
            request.Id.Should().Be("the-id");
            request.SerializationType.Should().Be(DocumentSerializationType.Dib);
            request.DefaultLanguage.Should().Be("csharp");
            request
                .Should()
                .BeOfType<NotebookSerializeRequest>();
            var serializeRequest = (NotebookSerializeRequest)request;
            serializeRequest.NewLine.Should().Be("\r\n");
            serializeRequest.Document.Should().BeEquivalentTo(new InteractiveDocument(new List<InteractiveDocumentElement>()
            {
                new InteractiveDocumentElement("csharp", "var x = 1;"),
                new InteractiveDocumentElement("fsharp", "let y = 2"),
            }));
        }

        [Fact]
        public void NotebookParseResponse_serialization_contract()
        {
            var _configuration = new Configuration()
                                 .UsingExtension(".json")
                                 .SetInteractive(Debugger.IsAttached);

            var response = new NotebookParseResponse("the-id", new InteractiveDocument(new List<InteractiveDocumentElement>()
            {
                new InteractiveDocumentElement("csharp", "var x = 1;")
            }));

            var json = response.ToJson();

            this.Assent(json, _configuration);
        }

        [Fact]
        public void NotebookSerializeResponse_serialization_contract()
        {
            var _configuration = new Configuration()
                                 .UsingExtension(".json")
                                 .SetInteractive(Debugger.IsAttached);

            var response = new NotebookSerializeResponse("the-id", new byte[] { 0x01, 0x02, 0x03 });

            var json = response.ToJson();

            this.Assent(json, _configuration);
        }

        [Fact]
        public void NotebookErrorResponse_serialization_contract()
        {
            var _configuration = new Configuration()
                                 .UsingExtension(".json")
                                 .SetInteractive(Debugger.IsAttached);

            var response = new NotebookErrorResponse("the-id", "some error message");

            var json = response.ToJson();

            this.Assent(json, _configuration);
        }

        private string GetTestFileContents(string extension = ".json", [CallerFilePath] string thisFilePath = null, [CallerMemberName] string testName = null)
        {
            var fileName = $"{GetType().Name}.{testName}.approved{extension}";
            var thisFileDirectory = Path.GetDirectoryName(thisFilePath);
            var fullFilePath = Path.Combine(thisFileDirectory, fileName);
            var contents = File.ReadAllText(fullFilePath);
            return contents;
        }
    }
}
