﻿using FluentAssertions;
using Moq;
using RockLib.Messaging.Testing;
using System;
using System.Net.Mime;
using Xunit;

namespace RockLib.Messaging.CloudEvents.Tests
{
    public class SequentialEventTests
    {
        [Fact(DisplayName = "Constructor 1 does not initialize anything")]
        public void Constructor1HappyPath()
        {
            var cloudEvent = new SequentialEvent();

            cloudEvent.Data.Should().BeNull();
            cloudEvent.AdditionalAttributes.Should().BeEmpty();
            cloudEvent.DataContentType.Should().BeNull();
            cloudEvent.DataSchema.Should().BeNull();
            cloudEvent.Id.Should().BeNull();
            cloudEvent.Sequence.Should().BeNull();
            cloudEvent.SequenceType.Should().BeNull();
            cloudEvent.Source.Should().BeNull();
            cloudEvent.SpecVersion.Should().Be("1.0");
            cloudEvent.Subject.Should().BeNull();
            cloudEvent.Time.Should().BeNull();
            cloudEvent.Type.Should().BeNull();
        }

        [Fact(DisplayName = "Constructor 2 initializes Data property")]
        public void Constructor2HappyPath()
        {
            var cloudEvent = new SequentialEvent("Hello, world!");

            cloudEvent.Data.Should().Be("Hello, world!");
            cloudEvent.AdditionalAttributes.Should().BeEmpty();
            cloudEvent.DataContentType.Should().BeNull();
            cloudEvent.DataSchema.Should().BeNull();
            cloudEvent.Id.Should().BeNull();
            cloudEvent.Sequence.Should().BeNull();
            cloudEvent.SequenceType.Should().BeNull();
            cloudEvent.Source.Should().BeNull();
            cloudEvent.SpecVersion.Should().Be("1.0");
            cloudEvent.Subject.Should().BeNull();
            cloudEvent.Time.Should().BeNull();
            cloudEvent.Type.Should().BeNull();
        }

        [Fact(DisplayName = "Constructor 3 initializes Data property")]
        public void Constructor3HappyPath()
        {
            var data = new byte[] { 1, 2, 3, 4 };

            var cloudEvent = new SequentialEvent(data);

            cloudEvent.Data.Should().BeSameAs(data);
            cloudEvent.AdditionalAttributes.Should().BeEmpty();
            cloudEvent.DataContentType.Should().BeNull();
            cloudEvent.DataSchema.Should().BeNull();
            cloudEvent.Id.Should().BeNull();
            cloudEvent.Sequence.Should().BeNull();
            cloudEvent.SequenceType.Should().BeNull();
            cloudEvent.Source.Should().BeNull();
            cloudEvent.SpecVersion.Should().Be("1.0");
            cloudEvent.Subject.Should().BeNull();
            cloudEvent.Time.Should().BeNull();
            cloudEvent.Type.Should().BeNull();
        }

        [Fact(DisplayName = "ToSenderMessage method maps sequential event attributes to sender message headers")]
        public void ToSenderMessageMethodHappyPath1()
        {
            // All attributes provided

            var sequential = "1";
            var sequentialType = SequenceTypes.Integer;
            var dataContentType = new ContentType("application/xml");
            var dataSchema = new Uri("http://dataschema");
            var id = "MyId";
            Uri source = new Uri("http://source");
            var subject = "MySubject";
            var time = DateTime.UtcNow;
            var type = "MyType";

            var cloudEvent = new SequentialEvent
            {
                Sequence = sequential,
                SequenceType = sequentialType,
                DataContentType = dataContentType,
                DataSchema = dataSchema,
                Id = id,
                Source = source,
                Subject = subject,
                Time = time,
                Type = type
            };

            var senderMessage = cloudEvent.ToSenderMessage();

            senderMessage.Headers[SequentialEvent.SequenceAttribute].Should().BeSameAs(sequential);
            senderMessage.Headers[SequentialEvent.SequenceTypeAttribute].Should().BeSameAs(sequentialType);

            senderMessage.Headers[CloudEvent.DataContentTypeAttribute].Should().Be(dataContentType.ToString());
            senderMessage.Headers[CloudEvent.DataSchemaAttribute].Should().Be(dataSchema.ToString());
            senderMessage.Headers[CloudEvent.IdAttribute].Should().Be(id);
            senderMessage.Headers[CloudEvent.SourceAttribute].Should().Be(source.ToString());
            senderMessage.Headers[CloudEvent.SpecVersionAttribute].Should().Be("1.0");
            senderMessage.Headers[CloudEvent.SubjectAttribute].Should().Be(subject);
            senderMessage.Headers[CloudEvent.TimeAttribute].Should().Be(time.ToString("O"));
            senderMessage.Headers[CloudEvent.TypeAttribute].Should().Be(type);
        }

        [Fact(DisplayName = "ToSenderMessage method does not map null sequential cloud event attributes to sender message headers")]
        public void ToSenderMessageMethodHappyPath2()
        {
            // No attributes provided

            var cloudEvent = new SequentialEvent();

            var senderMessage = cloudEvent.ToSenderMessage();

            senderMessage.Headers.Should().NotContainKey(SequentialEvent.SequenceAttribute);
            senderMessage.Headers.Should().NotContainKey(SequentialEvent.SequenceTypeAttribute);

            senderMessage.Headers.Should().NotContainKey(CloudEvent.DataContentTypeAttribute);
            senderMessage.Headers.Should().NotContainKey(CloudEvent.DataSchemaAttribute);
            senderMessage.Headers.Should().NotContainKey(CloudEvent.IdAttribute);
            senderMessage.Headers.Should().NotContainKey(CloudEvent.SourceAttribute);
            senderMessage.Headers[CloudEvent.SpecVersionAttribute].Should().Be("1.0");
            senderMessage.Headers.Should().NotContainKey(CloudEvent.SubjectAttribute);
            senderMessage.Headers.Should().NotContainKey(CloudEvent.TimeAttribute);
            senderMessage.Headers.Should().NotContainKey(CloudEvent.TypeAttribute);
        }

        [Fact(DisplayName = "ToSenderMessage method applies specified protocol binding to each attribute")]
        public void ToSenderMessageMethodHappyPath3()
        {
            // Non-default protocol binding

            var sequence = "1";
            var sequenceType = SequenceTypes.Integer;
            var dataContentType = new ContentType("application/xml");
            var dataSchema = new Uri("http://dataschema");
            var id = "MyId";
            Uri source = new Uri("http://source");
            var subject = "MySubject";
            var time = DateTime.UtcNow;
            var type = "MyType";

            var cloudEvent = new SequentialEvent
            {
                Sequence = sequence,
                SequenceType = sequenceType,
                DataContentType = dataContentType,
                DataSchema = dataSchema,
                Id = id,
                Source = source,
                Subject = subject,
                Time = time,
                Type = type
            };

            var mockProtocolBinding = new Mock<IProtocolBinding>();
            mockProtocolBinding.Setup(m => m.GetHeaderName(It.IsAny<string>())).Returns<string>(header => "test-" + header);

            var senderMessage = cloudEvent.ToSenderMessage(mockProtocolBinding.Object);

            senderMessage.Headers["test-" + SequentialEvent.SequenceAttribute].Should().BeSameAs(sequence);
            senderMessage.Headers["test-" + SequentialEvent.SequenceTypeAttribute].Should().BeSameAs(sequenceType);

            senderMessage.Headers["test-" + CloudEvent.DataContentTypeAttribute].Should().Be(dataContentType.ToString());
            senderMessage.Headers["test-" + CloudEvent.DataSchemaAttribute].Should().Be(dataSchema.ToString());
            senderMessage.Headers["test-" + CloudEvent.IdAttribute].Should().Be(id);
            senderMessage.Headers["test-" + CloudEvent.SourceAttribute].Should().Be(source.ToString());
            senderMessage.Headers["test-" + CloudEvent.SpecVersionAttribute].Should().Be("1.0");
            senderMessage.Headers["test-" + CloudEvent.SubjectAttribute].Should().Be(subject);
            senderMessage.Headers["test-" + CloudEvent.TimeAttribute].Should().Be(time.ToString("O"));
            senderMessage.Headers["test-" + CloudEvent.TypeAttribute].Should().Be(type);
        }

        [Fact(DisplayName = "Validate method does not throw when given valid sender message")]
        public void ValidateMethodHappyPath1()
        {
            var senderMessage = new SenderMessage("Hello, world!");
            
            senderMessage.Headers.Add(SequentialEvent.SequenceAttribute, "1");

            senderMessage.Headers.Add(CloudEvent.SpecVersionAttribute, "1.0");
            senderMessage.Headers.Add(CloudEvent.IdAttribute, "MyId");
            senderMessage.Headers.Add(CloudEvent.SourceAttribute, new Uri("http://MySource"));
            senderMessage.Headers.Add(CloudEvent.TypeAttribute, "MyType");
            senderMessage.Headers.Add(CloudEvent.TimeAttribute, DateTime.UtcNow);

            Action act = () => SequentialEvent.Validate(senderMessage);

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "Validate method does not throw when given valid sender message for specified protocol binding")]
        public void ValidateMethodHappyPath2()
        {
            // Non-default protocol binding

            var senderMessage = new SenderMessage("Hello, world!");

            senderMessage.Headers.Add("test-" + SequentialEvent.SequenceAttribute, "1");

            senderMessage.Headers.Add("test-" + CloudEvent.SpecVersionAttribute, "1.0");
            senderMessage.Headers.Add("test-" + CloudEvent.IdAttribute, "MyId");
            senderMessage.Headers.Add("test-" + CloudEvent.SourceAttribute, new Uri("http://MySource"));
            senderMessage.Headers.Add("test-" + CloudEvent.TypeAttribute, "MyType");
            senderMessage.Headers.Add("test-" + CloudEvent.TimeAttribute, DateTime.UtcNow);

            var mockProtocolBinding = new Mock<IProtocolBinding>();
            mockProtocolBinding.Setup(m => m.GetHeaderName(It.IsAny<string>())).Returns<string>(header => "test-" + header);

            Action act = () => SequentialEvent.Validate(senderMessage, mockProtocolBinding.Object);

            act.Should().NotThrow();
        }

        [Fact(DisplayName = "Validate method throws given missing Sequence header")]
        public void ValidateMethodSadPath()
        {
            // Missing Sequence

            var senderMessage = new SenderMessage("Hello, world!");

            senderMessage.Headers.Add(CloudEvent.IdAttribute, "MyId");
            senderMessage.Headers.Add(CloudEvent.SourceAttribute, new Uri("http://MySource"));
            senderMessage.Headers.Add(CloudEvent.TypeAttribute, "MyType");
            senderMessage.Headers.Add(CloudEvent.TimeAttribute, DateTime.UtcNow);

            Action act = () => SequentialEvent.Validate(senderMessage);

            act.Should().ThrowExactly<CloudEventValidationException>();
        }

        [Fact(DisplayName = "Create method maps sequential event attributes from receiver message headers")]
        public void CreateMethodHappyPath1()
        {
            // All attributes provided

            var receiverMessage = new FakeReceiverMessage("Hello, world!");

            var source = new Uri("http://MySource");
            var dataContentType = new ContentType("application/mycontenttype");
            var dataSchema = new Uri("http://MySource");
            var time = DateTime.UtcNow;

            receiverMessage.Headers.Add(SequentialEvent.SequenceAttribute, "1");
            receiverMessage.Headers.Add(SequentialEvent.SequenceTypeAttribute, SequenceTypes.Integer);

            receiverMessage.Headers.Add(CloudEvent.IdAttribute, "MyId");
            receiverMessage.Headers.Add(CloudEvent.SourceAttribute, source);
            receiverMessage.Headers.Add(CloudEvent.TypeAttribute, "MyType");
            receiverMessage.Headers.Add(CloudEvent.DataContentTypeAttribute, dataContentType);
            receiverMessage.Headers.Add(CloudEvent.DataSchemaAttribute, dataSchema);
            receiverMessage.Headers.Add(CloudEvent.SubjectAttribute, "MySubject");
            receiverMessage.Headers.Add(CloudEvent.TimeAttribute, time);

            var sequentialEvent = SequentialEvent.Create(receiverMessage);

            sequentialEvent.Sequence.Should().Be("1");
            sequentialEvent.SequenceType.Should().Be(SequenceTypes.Integer);

            sequentialEvent.Id.Should().Be("MyId");
            sequentialEvent.Source.Should().BeSameAs(source);
            sequentialEvent.Type.Should().Be("MyType");
            sequentialEvent.DataContentType.Should().BeSameAs(dataContentType);
            sequentialEvent.DataSchema.Should().BeSameAs(dataSchema);
            sequentialEvent.Subject.Should().Be("MySubject");
            sequentialEvent.Time.Should().Be(time);
            sequentialEvent.AdditionalAttributes.Should().BeEmpty();
        }

        [Fact(DisplayName = "Create method does not require any sequential event attributes to be mapped")]
        public void CreateMethodHappyPath2()
        {
            // No attributes provided

            var receiverMessage = new FakeReceiverMessage("Hello, world!");

            var sequentialEvent = SequentialEvent.Create(receiverMessage);

            sequentialEvent.Sequence.Should().BeNull();
            sequentialEvent.SequenceType.Should().BeNull();

            sequentialEvent.Id.Should().BeNull();
            sequentialEvent.Source.Should().BeNull();
            sequentialEvent.Type.Should().BeNull();
            sequentialEvent.DataContentType.Should().BeNull();
            sequentialEvent.DataSchema.Should().BeNull();
            sequentialEvent.Subject.Should().BeNull();
            sequentialEvent.Time.Should().BeNull();
            sequentialEvent.AdditionalAttributes.Should().BeEmpty();
        }

        [Fact(DisplayName = "Create method maps with the specified protocol binding")]
        public void CreateMethodHappyPath3()
        {
            // Non-default protocol binding

            var receiverMessage = new FakeReceiverMessage("Hello, world!");

            var source = new Uri("http://MySource");
            var dataContentType = new ContentType("application/mycontenttype");
            var dataSchema = new Uri("http://MySource");
            var time = DateTime.UtcNow;

            receiverMessage.Headers.Add("test-" + SequentialEvent.SequenceAttribute, "1");
            receiverMessage.Headers.Add("test-" + SequentialEvent.SequenceTypeAttribute, SequenceTypes.Integer);

            receiverMessage.Headers.Add("test-" + CloudEvent.IdAttribute, "MyId");
            receiverMessage.Headers.Add("test-" + CloudEvent.SourceAttribute, source);
            receiverMessage.Headers.Add("test-" + CloudEvent.TypeAttribute, "MyType");
            receiverMessage.Headers.Add("test-" + CloudEvent.DataContentTypeAttribute, dataContentType);
            receiverMessage.Headers.Add("test-" + CloudEvent.DataSchemaAttribute, dataSchema);
            receiverMessage.Headers.Add("test-" + CloudEvent.SubjectAttribute, "MySubject");
            receiverMessage.Headers.Add("test-" + CloudEvent.TimeAttribute, time);

            var mockProtocolBinding = new Mock<IProtocolBinding>();
            mockProtocolBinding.Setup(m => m.GetHeaderName(It.IsAny<string>())).Returns<string>(header => "test-" + header);

            var sequentialEvent = SequentialEvent.Create(receiverMessage, mockProtocolBinding.Object);

            sequentialEvent.Sequence.Should().Be("1");
            sequentialEvent.SequenceType.Should().Be(SequenceTypes.Integer);

            sequentialEvent.Id.Should().Be("MyId");
            sequentialEvent.Source.Should().BeSameAs(source);
            sequentialEvent.Type.Should().Be("MyType");
            sequentialEvent.DataContentType.Should().BeSameAs(dataContentType);
            sequentialEvent.DataSchema.Should().BeSameAs(dataSchema);
            sequentialEvent.Subject.Should().Be("MySubject");
            sequentialEvent.Time.Should().Be(time);
            sequentialEvent.AdditionalAttributes.Should().BeEmpty();
        }
    }
}