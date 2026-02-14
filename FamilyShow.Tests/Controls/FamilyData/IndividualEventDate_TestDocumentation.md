# IndividualEventDate Control Test Suite

## Overview
This document describes the comprehensive test suite created for the `IndividualEventDate` WPF UserControl, which is used for editing date events with descriptors in the Family.Show genealogy application.

## Test Statistics
- **Total Tests**: 42
- **Test Categories**: 5
- **Coverage Areas**: Dependency Properties, Event Handlers, Helper Methods, Integration Scenarios, Edge Cases

## Test Structure

### 1. Dependency Property Tests (14 tests)
Tests all dependency properties to ensure they work correctly with WPF's binding system:

#### Date Property
- `Date_PropertySetGet_ShouldWorkCorrectly()` - Verifies date setting and retrieval
- `Date_InitialValue_ShouldBeNull()` - Confirms default state

#### DateDescriptor Property
- `DateDescriptor_PropertySetGet_ShouldWorkCorrectly()` - Tests descriptor string handling
- `DateDescriptor_InitialValue_ShouldBeEmpty()` - Verifies default empty state

#### DateLabelText Property  
- `DateLabelText_PropertySetGet_ShouldWorkCorrectly()` - Tests label text assignment
- `DateLabelText_DefaultValue_ShouldBeDate()` - Confirms default "Date" text

#### IsControlEnabled Property
- `IsControlEnabled_PropertySetGet_ShouldWorkCorrectly()` - Tests inverted logic implementation

#### ShowCheckBox Property
- `ShowCheckBox_PropertySetGet_ShouldWorkCorrectly()` - Tests visibility toggle
- `ShowCheckBox_DefaultValue_ShouldBeFalse()` - Verifies default hidden state

#### CheckBox Properties  
- `CheckBoxText_PropertySetGet_ShouldWorkCorrectly()` - Tests checkbox label
- `CheckBoxText_DefaultValue_ShouldBeLiving()` - Confirms default "Living" text
- `CheckBoxValue_PropertySetGet_ShouldWorkCorrectly()` - Tests checkbox state
- `CheckBoxValue_DefaultValue_ShouldBeTrue()` - Verifies default checked state
- `CheckBoxValue_CanBeNull_ShouldWorkCorrectly()` - Tests nullable behavior

### 2. Event Handler Tests (5 tests)
Tests user interaction event handling:

#### Routed Events
- `DateDescriptorChangedEvent_WhenRaised_ShouldBeHandled()` - Tests custom event raising

#### Mouse Events
- `Label_MouseEnter_WithEnabledLabel_ShouldSetHandCursor()` - Tests cursor change on hover
- `Label_MouseEnter_WithDisabledLabel_ShouldNotSetCursor()` - Tests disabled state behavior
- `Label_MouseLeave_ShouldResetCursor()` - Tests cursor restoration

#### Tooltip Events
- `ToolTip_DateTextBox_ShouldSetHelpfulTooltip()` - Tests tooltip content setting

### 3. Date Descriptor Helper Method Tests (16 tests)
Tests the static helper methods for cycling through date descriptors:

#### Forward Cycling (8 tests)
- Tests progression: "" ? "abt " ? "bef " ? "aft " ? "bet " ? "cal " ? "est " ? ""
- Tests unknown values return empty string

#### Backward Cycling (8 tests)  
- Tests regression: "" ? "abt " ? "bef " ? "aft " ? "bet " ? "cal " ? "est " ? ""
- Tests unknown values return empty string

### 4. Integration Tests (4 tests)
Tests real-world usage scenarios:

#### Scenario Testing
- `Control_WithBirthDateScenario_ShouldConfigureCorrectly()` - Tests birth date configuration
- `Control_WithDeathDateScenario_ShouldConfigureCorrectly()` - Tests death date configuration
- `Control_DateDescriptorCycling_ShouldWorkBothDirections()` - Tests bidirectional cycling
- `Control_InitialState_ShouldBeValid()` - Tests default control state

### 5. Edge Case Tests (3 tests)
Tests boundary conditions and error scenarios:

#### Null Handling
- `DateDescriptor_WithNullValue_ShouldHandleGracefully()` - Tests null input handling
- `Date_WithInvalidDate_ShouldHandleGracefully()` - Tests invalid date handling

#### State Management
- `CheckBoxValue_WithMultipleChanges_ShouldMaintainConsistency()` - Tests state transitions

## Key Features Tested

### Date Descriptor Cycling
The control supports cycling through GEDCOM-standard date descriptors:
- **Forward**: Empty ? "abt " ? "bef " ? "aft " ? "bet " ? "cal " ? "est " ? Empty
- **Backward**: Empty ? "abt " ? "bef " ? "aft " ? "bet " ? "cal " ? "est " ? Empty

### Property Binding
All dependency properties are tested for:
- Correct getter/setter behavior
- Default value initialization  
- Data binding compatibility
- Type safety

### User Interaction
Event handlers are tested for:
- Mouse cursor management
- Tooltip functionality
- Custom routed events
- UI state changes

### Control Configuration
Different usage scenarios tested:
- Birth date entry (with "Living" checkbox)
- Death date entry (without checkbox)
- Various date descriptor combinations
- Enabled/disabled states

## Test Attributes Used

### `[StaFact]`
Used for tests requiring Single Threaded Apartment mode (WPF controls):
- All dependency property tests
- Event handler tests  
- Integration tests
- Edge case tests requiring control instantiation

### `[Fact]`
Used for tests not requiring STA mode:
- Static helper method tests
- Pure logic tests

## Test Implementation Notes

### Reflection Usage
Private static methods are tested using reflection:
```csharp
var method = typeof(IndividualEventDate).GetMethod("ForwardDateDescriptor", 
    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
```

### WPF Control Testing
Tests handle WPF-specific requirements:
- STA thread apartment state
- Dependency property metadata
- Event routing
- UI element interaction

### Inverted Logic Handling
The `IsControlEnabled` property uses inverted logic (disabled = true):
```csharp
public bool IsControlEnabled
{
    get { return !(bool)GetValue(IsControlEnabledProperty); }
    set { SetValue(IsControlEnabledProperty, !value); }
}
```
Tests account for this design decision.

## Coverage Summary

The test suite provides comprehensive coverage of:
- ? All public dependency properties
- ? All event handlers (public and private)  
- ? All helper methods (static and instance)
- ? Integration scenarios for typical use cases
- ? Edge cases and error conditions
- ? Default values and initial state
- ? Property change notifications
- ? WPF-specific behavior

## Running the Tests

Execute the test suite using:
```bash
dotnet test FamilyShow.Tests/FamilyShow.Tests.csproj --filter "IndividualEventDateTest"
```

All 42 tests pass successfully, providing confidence in the control's reliability and correctness.
