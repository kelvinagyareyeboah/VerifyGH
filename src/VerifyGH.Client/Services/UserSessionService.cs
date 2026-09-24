using VerifyGH.Shared.Enums;

namespace VerifyGH.Client.Services;

public class UserSession
{
    public string Name { get; set; } = "Kelvin Yeboah";
    public string Email { get; set; } = "kelvin@st.ug.edu.gh";
    public string Organization { get; set; } = "University of Ghana";
    public UserRole Role { get; set; } = UserRole.Student;
    public bool IsAuthenticated { get; set; } = true;
}

public class UserSessionService
{
    private UserSession _currentSession = new()
    {
        Name = "Kelvin Yeboah",
        Email = "kelvin@st.ug.edu.gh",
        Organization = "University of Ghana",
        Role = UserRole.Student,
        IsAuthenticated = true
    };

    public UserSession CurrentSession => _currentSession;
    public UserRole CurrentRole => _currentSession.Role;
    public bool IsAuthenticated => _currentSession.IsAuthenticated;

    public event Action? OnChange;

    public void SwitchRole(UserRole role)
    {
        _currentSession.Role = role;
        _currentSession.IsAuthenticated = true;

        switch (role)
        {
            case UserRole.Student:
                _currentSession.Name = "Kelvin Yeboah";
                _currentSession.Email = "kelvin@st.ug.edu.gh";
                _currentSession.Organization = "University of Ghana";
                break;
            case UserRole.Lecturer:
                _currentSession.Name = "Dr. Michael Soli";
                _currentSession.Email = "msoli@ug.edu.gh";
                _currentSession.Organization = "Dept. of Computer Science, UG";
                break;
            case UserRole.Employer:
                _currentSession.Name = "Kwadwo Mensah";
                _currentSession.Email = "talent@hubtel.com";
                _currentSession.Organization = "Hubtel Ghana";
                break;
            default:
                _currentSession.Name = "System Administrator";
                _currentSession.Email = "admin@verifygh.edu.gh";
                _currentSession.Organization = "VerifyGH Admin";
                break;
        }

        NotifyStateChanged();
    }

    public void Login(string email, UserRole role, string? name = null, string? organization = null)
    {
        _currentSession.Email = email;
        _currentSession.Role = role;
        _currentSession.IsAuthenticated = true;

        if (!string.IsNullOrEmpty(name))
            _currentSession.Name = name;
        else
            _currentSession.Name = role == UserRole.Student ? "Student User" : role == UserRole.Lecturer ? "Faculty Reviewer" : "Recruiter";

        if (!string.IsNullOrEmpty(organization))
            _currentSession.Organization = organization;

        NotifyStateChanged();
    }

    public void Logout()
    {
        _currentSession.IsAuthenticated = false;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
