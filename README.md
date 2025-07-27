# MCP_PDF
MCP to serve a Html Page or an JSON array as PDF
For .NET 10:

{
  "servers": {
    "MCP_Array_Any": {
      "type": "stdio",
      "command": "dnx",
      "args": ["MCPArray2Any", "--yes"],
      "env": {
        "logMCPFile": "D:\\mcp-pdf-.log",
      }
    }
  }
}


For  .NET 8:

Install 
```cmd
dotnet tool install --global MCPArray2Any --version 8.2025.727.1559
```
And put into .vscode/mcp.json

```json
{
  "servers": {
    "MCP_Array_Any": {
      "type": "stdio",
      "command": "MCP_PDF",       
      "args": [
      ],
      "env": {
        "logMCPFile": "D:\\mcp-pdf-.log",
      }
    },
     "filesystem": {
      "command": "npx",
      "args": [
        "-y",
        "@modelcontextprotocol/server-filesystem",
        "${workspaceFolder}",
        "${workspaceFolder}/output"
        
      ]
    },
    "sequential-thinking": {
      "command": "npx",
      "args": [
        "-y",
        "@modelcontextprotocol/server-sequential-thinking"
      ]
    }
  }
}
```
# Prompts
Prompt 1:

Please convert this json into html : [
{"firstName":"John", "lastName":"Doe"},
{"firstName":"Anna", "lastName":"Smith"},
{"firstName":"Peter", "lastName":"Jones"}
] and store in a local file


Prompt 2:

Use MCP file system and  to find the current folder . 

Please convert this json into pdf : [
{"firstName":"John", "lastName":"Doe"},
{"firstName":"Anna", "lastName":"Smith"},
{"firstName":"Peter", "lastName":"Jones"}s
] by saving to result.pdf in the current folder 

Prompt 3:

Use MCP file system and  to find the current folder . 

Please fetch data from 
https://jsonplaceholder.typicode.com/posts

and save to posts.pdf in the current folder