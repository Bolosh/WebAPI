using ClassLibrary.Domain;
using ClassLibrary.IRepository;
using Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace WebApiTests;

[TestClass]
public class PatientControllerTest
{
    [TestMethod]
    public async Task GetAllPatients_WithValidController_ReturnsListOfPatients()
    {
        // Arrange
        var _mock_repo = new Mock<IPatientRepository>();
        var _fake_patients = new List<Patient>
        {
            new Patient { UserId = 1, BloodType = "AB+", EmergencyContact = "Mom", Allergies = "cats, dogs", Weight = 72.6, Height = 167 },
            new Patient { UserId = 2, BloodType = "A-", EmergencyContact = "Dad", Allergies = "cats, dogs", Weight = 92.6, Height = 199 }
        };
        _mock_repo.Setup(_repo => _repo.GetAllPatientsAsync()).ReturnsAsync(_fake_patients);

        var _controller = new PatientController(_mock_repo.Object);

        // Act
        var _result = await _controller.GetAllPatients();
        var _ok_result = _result.Result as OkObjectResult;

        // Assert
        Assert.IsNotNull(_ok_result);
        var _returned_patients = _ok_result.Value as List<Patient>;
        Assert.AreEqual(2, _returned_patients.Count);
    }

    [TestMethod]
    public async Task GetPatientById_WithValidPatientId_ReturnsPatient()
    {
        // Arrange
        var _patient_id = 2;
        var _mock_repo = new Mock<IPatientRepository>();
        var _fake_patient = new Patient { UserId = 2, BloodType = "A-", EmergencyContact = "Dad", Allergies = "cats, dogs", Weight = 92.6, Height = 199 };

        _mock_repo.Setup(_repo => _repo.GetPatientByUserIdAsync(_patient_id)).ReturnsAsync(_fake_patient);

        var _controller = new PatientController(_mock_repo.Object);

        // Act
        var _result = await _controller.GetPatientById(_patient_id);
        var _ok_result = _result.Result as OkObjectResult;

        // Assert
        Assert.IsNotNull(_ok_result);
        var _returned_patient = _ok_result.Value as Patient;
        Assert.AreEqual(2, _returned_patient.UserId);
    }

    [TestMethod]
    public async Task CreatePatient_WithValidPatient_ReturnsCreatedAtAction()
    {
        // Arrange
        var _mock_repo = new Mock<IPatientRepository>();
        var _fake_patient = new Patient { UserId = 2, BloodType = "A-", EmergencyContact = "Dad", Allergies = "cats, dogs", Weight = 92.6, Height = 199 };
        _mock_repo.Setup(repo => repo.AddPatientAsync(_fake_patient)).Returns(Task.CompletedTask);

        var _controller = new PatientController(_mock_repo.Object);

        // Act
        var _result = await _controller.CreatePatient(_fake_patient);
        var _created_at = _result as CreatedAtActionResult;

        // Assert
        Assert.IsNotNull(_created_at);
        Assert.AreEqual(_fake_patient.UserId, ((Patient)_created_at.Value).UserId);
    }

    [TestMethod]
    public async Task DeletePatient_WithValidPatientId_ReturnsNoContent()
    {
        // Arrange
        var _mock_repo = new Mock<IPatientRepository>();
        int _patient_id = 1;

        // No setup needed if the method succeeds (completes without exception)
        _mock_repo.Setup(_repo => _repo.DeletePatientAsync(_patient_id)).Returns(Task.CompletedTask);

        var _controller = new PatientController(_mock_repo.Object);

        // Act
        var _result = await _controller.DeletePatient(_patient_id);

        // Assert
        Assert.IsInstanceOfType(_result, typeof(NoContentResult));
        _mock_repo.Verify(_repo => _repo.DeletePatientAsync(_patient_id), Times.Once);
    }
}
