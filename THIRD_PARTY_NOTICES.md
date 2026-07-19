# Third-Party Notices

## sparxeng.com — Serial port read pattern

The `ReadData` method in `Serial.cs` is adapted from a technique described in:

> **"Must-use .NET System.IO.Ports.SerialPort"**
> https://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport

This blog post describes the correct asynchronous approach for reading from
`System.IO.Ports.SerialPort` in .NET to avoid data loss. The adaptation is
credited inline in the source file.

No licence was stated on the blog post. Attribution is provided as a courtesy.
The remaining code in this repository is original work by VaderConsulting and
is released under the MIT Licence.