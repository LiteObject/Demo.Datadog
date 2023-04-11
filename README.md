# Loggin & Datadog with .NET 6

---
## What is DataDog?
>Datadog is a monitoring and analytics tool that can be used to determine performance metrics as well as event monitoring for infrastructure and cloud services. 
---
## What is Serilog?
>Serilog is a logging library that supports of _structured logging_, which allows logs to be treated as data sets rather than text. 
---
## Flow diagram

```mermaid
flowchart LR
        classDef appstyle fill:#1995AD,stroke:#fff,stroke-width:1px;
        classDef opentelstyle fill:#F34A4A,stroke:#fff,stroke-width:1px;    
        classDef ddstyle fill:#C99E10,stroke:#fff,stroke-width:1px;

        subgraph .NET Applications
        C1(Controller):::appstyle --> SL(Serilog):::opentelstyle
        C2(Domain Service):::appstyle --> SL(Serilog):::opentelstyle 
        C3(Data Access):::appstyle --> SL(Serilog):::opentelstyle
        end

        subgraph Sinks
        SL(Serilog):::opentelstyle -->|Export Logs| DD(DataDog):::ddstyle
        SL(Serilog):::opentelstyle -->|Export Logs| SEQ(Seq):::ddstyle
        SL(Serilog):::opentelstyle -->|Export Logs| SLK(Slack):::ddstyle
        SL(Serilog):::opentelstyle -->|Export Logs| NR(NewRelic):::ddstyle
        SL(Serilog):::opentelstyle -->|Export Logs| RMQ(RabbitMQ):::ddstyle        
        end
```

---
## Open-Source Alternatives:
* Zipkin
* Jaeger
* Signoz
---
## Links:
* [Serilog: Getting Started](https://github.com/serilog/serilog/wiki/Getting-Started)
* [Serilog: Configuration Basics](https://github.com/serilog/serilog/wiki/Configuration-Basics)
* [List of Serilog Sinks](https://github.com/serilog/serilog/wiki/Provided-Sinks)
* [Datadog Integration with .NET](https://docs.datadoghq.com/integrations/dotnet/)
* [DataDog/serilog-sinks-datadog-logs](https://github.com/DataDog/serilog-sinks-datadog-logs)
* [Datalust/serilog-sinks-seq](https://github.com/datalust/serilog-sinks-seq)

