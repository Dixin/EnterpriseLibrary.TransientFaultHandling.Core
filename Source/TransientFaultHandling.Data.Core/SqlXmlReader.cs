﻿namespace Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

using System.Data;
using System.Xml;

/// <summary>
/// Provides a disposable wrapper for SQL XML data reader, which synchronizes the SQL connection
/// disposal with its own life cycle.
/// </summary>
internal class SqlXmlReader : XmlReader
{
    private readonly IDbConnection connection;

    private readonly XmlReader innerReader;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlXmlReader"/> class that is associated with the specified SQL connection and the original XML reader.
    /// </summary>
    /// <param name="connection">The SQL connection that provides access to the XML data for this reader.</param>
    /// <param name="innerReader">The original XML reader that is to be wrapped by this instance.</param>
    public SqlXmlReader(IDbConnection connection, XmlReader innerReader) => 
        (this.connection, this.innerReader) = (connection.ThrowIfNull(), innerReader.ThrowIfNull());

    /// <summary>
    /// Returns the number of attributes on the current node.
    /// </summary>
    public override int AttributeCount => this.innerReader.AttributeCount;

    /// <summary>
    /// Returns the base Uniform Resource Identifier (URI) of the current node.
    /// </summary>
    public override string? BaseURI => this.innerReader.BaseURI;

    /// <summary>
    /// Returns the depth of the current node in the XML document.
    /// </summary>
    public override int Depth => this.innerReader.Depth;

    /// <summary>
    /// Returns a value that indicates whether the reader is positioned at the end of the stream.
    /// </summary>
    public override bool EOF => this.innerReader.EOF;

    /// <summary>
    /// Returns a value that indicates whether the current node can have a <see cref="System.Xml.XmlReader.Value"/>.
    /// </summary>
    public override bool HasValue => this.innerReader.HasValue;

    /// <summary>
    /// Returns a value that indicates whether the current node is an empty element.
    /// </summary>
    public override bool IsEmptyElement => this.innerReader.IsEmptyElement;

    /// <summary>
    /// Returns the local name of the current node.
    /// </summary>
    public override string LocalName => this.innerReader.LocalName;

    /// <summary>
    /// Returns the <see cref="System.Xml.XmlNameTable"/> that is associated with this implementation.
    /// </summary>
    public override XmlNameTable NameTable => this.innerReader.NameTable;

    /// <summary>
    /// Returns the namespace URI (as defined in the W3C Namespace specification) of the node on which the reader is positioned.
    /// </summary>
    public override string NamespaceURI => this.innerReader.NamespaceURI;

    /// <summary>
    /// Returns the type of the current node.
    /// </summary>
    public override XmlNodeType NodeType => this.innerReader.NodeType;

    /// <summary>
    /// Returns the namespace prefix that is associated with the current node.
    /// </summary>
    public override string Prefix => this.innerReader.Prefix;

    /// <summary>
    /// Returns the state of the reader.
    /// </summary>
    public override ReadState ReadState => this.innerReader.ReadState;

    /// <summary>
    /// Returns the text value of the current node.
    /// </summary>
    public override string Value => this.innerReader.Value;

    /// <summary>
    /// Returns the value of the attribute that has the specified name.
    /// </summary>
    /// <param name="name">The qualified name of the attribute.</param>
    /// <returns>The value of the specified attribute, or null if the attribute isn't found or its value is <see cref="String.Empty"/>.</returns>
    public override string? GetAttribute(string name) => this.innerReader.GetAttribute(name);

    /// <summary>
    /// Returns the value of the attribute that has the specified index.
    /// </summary>
    /// <param name="i">The index of the attribute. The index is zero-based. (The first attribute has an index of 0.)</param>
    /// <returns>The value of the specified attribute. This method does not move the reader.</returns>
    public override string GetAttribute(int i) => this.innerReader.GetAttribute(i);

    /// <summary>
    /// Returns the value of the attribute that has the specified name and namespace URI.
    /// </summary>
    /// <param name="name">The local name of the attribute.</param>
    /// <param name="namespaceUri">The namespace URI of the attribute.</param>
    /// <returns>The value of the specified attribute, or null if the attribute isn't found or its value is <see cref="String.Empty"/>. This method does not move the reader.</returns>
    public override string? GetAttribute(string name, string? namespaceUri) => this.innerReader.GetAttribute(name, namespaceUri);

    /// <summary>
    /// Closes both the original <see cref="System.Xml.XmlReader"/> and the associated SQL connection.
    /// </summary>
    public override void Close()
    {
        this.innerReader.Close();

        this.connection.Close();
    }

    /// <summary>
    /// Resolves a namespace prefix in the current element's scope.
    /// </summary>
    /// <param name="prefix">The prefix whose namespace URI you want to resolve. To match the default namespace, pass an empty string.</param>
    /// <returns>The namespace URI to which the prefix maps, or null if no matching prefix is found.</returns>
    public override string? LookupNamespace(string prefix) => this.innerReader.LookupNamespace(prefix);

    /// <summary>
    /// Moves to the attribute that has the specified name and namespace URI.
    /// </summary>
    /// <param name="name">The local name of the attribute.</param>
    /// <param name="ns">The namespace URI of the attribute.</param>
    /// <returns>true if the attribute is found; otherwise, false. If false, the reader's position does not change.</returns>
    public override bool MoveToAttribute(string name, string? ns) => this.innerReader.MoveToAttribute(name, ns);

    /// <summary>
    /// Moves to the attribute that has the specified name.
    /// </summary>
    /// <param name="name">The qualified name of the attribute.</param>
    /// <returns>true if the attribute is found; otherwise, false. If false, the reader's position does not change.</returns>
    public override bool MoveToAttribute(string name) => this.innerReader.MoveToAttribute(name);

    /// <summary>
    /// Moves to the element that contains the current attribute node.
    /// </summary>
    /// <returns>true if the reader is positioned on an attribute (in which case, the reader moves to the element that owns the attribute); false if the reader is not positioned on an attribute (in which case, the position of the reader does not change).</returns>
    public override bool MoveToElement() => this.innerReader.MoveToElement();

    /// <summary>
    /// Moves to the first attribute.
    /// </summary>
    /// <returns>true if an attribute exists (in which case, the reader moves to the first attribute); otherwise, false (in which case, the position of the reader does not change).</returns>
    public override bool MoveToFirstAttribute() => this.innerReader.MoveToFirstAttribute();

    /// <summary>
    /// Moves to the next attribute.
    /// </summary>
    /// <returns>true if there is a next attribute; false if there are no more attributes.</returns>
    public override bool MoveToNextAttribute() => this.innerReader.MoveToNextAttribute();

    /// <summary>
    /// Reads the next node from the stream.
    /// </summary>
    /// <returns>true if the next node was read successfully; false if there are no more nodes to read.</returns>
    public override bool Read() => this.innerReader.Read();

    /// <summary>
    /// Parses the attribute value into one or more Text, EntityReference, or EndEntity nodes.
    /// </summary>
    /// <returns>true if there are nodes to return, false if the reader is not positioned on an attribute node when the initial call is made or if all the attribute values have been read. An empty attribute such as misc="" returns true with a single node that has a value of <see cref="String.Empty"/>.</returns>
    public override bool ReadAttributeValue() => this.innerReader.ReadAttributeValue();

    /// <summary>
    /// Resolves the entity reference for EntityReference nodes.
    /// </summary>
    public override void ResolveEntity() => this.innerReader.ResolveEntity();
}