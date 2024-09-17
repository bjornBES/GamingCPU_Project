# CPU Specifications

- [CPU Specifications](#cpu-specifications)
  - [Interrupt vector table](#interrupt-vector-table)
    - [Layout](#layout)
    - [Interrupt vector assignments](#interrupt-vector-assignments)
      - [interrupt](#interrupt)
      - [exception](#exception)
  - [Pipelining](#pipelining)
  - [Caches](#caches)
  - [Calling convention](#calling-convention)
    - [Caller](#caller)
    - [Callee](#callee)
  - [MEMORY](#memory)
    - [MEMORY LAYOUT](#memory-layout)
  - [REGISTERS](#registers)

## Interrupt vector table

### Layout

### Interrupt vector assignments

#### interrupt

#### exception

## Pipelining

The BCG arch can do 3 stage pipelining like this

|Stage      |Stage1             |Stage2               |Stage3                 |Stage4
|-----------|-------------------|---------------------|-----------------------|-
|Opertion1  |Fetch instruction  |instruction Decoding |instruction execution  |Fetch instruction
|Opertion2  |                   |Fetch instruction    |instruction Decoding   |instruction execution
|Opertion2  |                   |                     |Fetch instruction      |instruction Decoding

## Caches

## Calling convention

### Caller

### Callee

## MEMORY

### MEMORY LAYOUT

## REGISTERS
