// script.js

document.addEventListener('DOMContentLoaded', function() {
    // Initialize the dashboard
    initDashboard();
});

function initDashboard() {
    // Initialize all components
    initSidebar();
    initNavigation();
    initModals();
    initForms();
    initNotifications();
    initPasswordToggle();
    initStatusToggle();
    initPayment();
    updateDateTime();
    
    // Set up periodic updates
    setInterval(updateDateTime, 60000); // Update time every minute
}

// Navigation functionality
function initNavigation() {
    const navItems = document.querySelectorAll('.nav-item');
    const pageSections = document.querySelectorAll('.page-section');
    const pageTitle = document.getElementById('pageTitle');
    
    // Page titles mapping
    const pageTitles = {
        'dashboard': 'Tenant Dashboard',
        'profile': 'Tenant Profile',
        'payments': 'My Payments',
        'request': 'Request / Complaint'
    };
    
    navItems.forEach(item => {
        item.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Remove active class from all nav items and sections
            navItems.forEach(nav => nav.classList.remove('active'));
            pageSections.forEach(section => section.classList.remove('active'));
            
            // Add active class to clicked nav item
            this.classList.add('active');
            
            // Show corresponding section
            const target = this.getAttribute('data-target');
            const targetSection = document.getElementById(target);
            
            if (targetSection) {
                targetSection.classList.add('active');
                
                // Update page title
                if (pageTitle && pageTitles[target]) {
                    pageTitle.textContent = pageTitles[target];
                }
                
                // Scroll to top
                window.scrollTo(0, 0);
            }
        });
    });
}

// Sidebar functionality
function initSidebar() {
    const menuToggle = document.getElementById('menuToggle');
    const sidebar = document.querySelector('.sidebar');
    const mainContent = document.querySelector('.main-content');
    
    if (menuToggle && sidebar) {
        menuToggle.addEventListener('click', function() {
            sidebar.classList.toggle('collapsed');
            mainContent.classList.toggle('sidebar-collapsed');
        });
    }
    
    // Handle window resize
    window.addEventListener('resize', function() {
        if (window.innerWidth <= 768) {
            sidebar.classList.add('collapsed');
            mainContent.classList.add('sidebar-collapsed');
        } else {
            sidebar.classList.remove('collapsed');
            mainContent.classList.remove('sidebar-collapsed');
        }
    });
    
    // Initialize responsive sidebar on load
    if (window.innerWidth <= 768) {
        sidebar.classList.add('collapsed');
        mainContent.classList.add('sidebar-collapsed');
    }
    
    // Add click outside to close sidebar on mobile
    document.addEventListener('click', function(event) {
        if (window.innerWidth <= 768 && 
            sidebar && !sidebar.contains(event.target) && 
            menuToggle && !menuToggle.contains(event.target) &&
            !sidebar.classList.contains('collapsed')) {
            sidebar.classList.add('collapsed');
            mainContent.classList.add('sidebar-collapsed');
        }
    });
}

// Modal functionality
function initModals() {
    const passwordModal = document.getElementById('passwordModal');
    const updatePasswordBtn = document.querySelector('.update-password-btn');
    const closeModalBtns = document.querySelectorAll('.close-modal');
    
    // Open password modal
    if (updatePasswordBtn) {
        updatePasswordBtn.addEventListener('click', function() {
            openModal(passwordModal);
        });
    }
    
    // Close modal functionality
    closeModalBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            const modal = this.closest('.modal');
            closeModal(modal);
        });
    });
    
    // Close modal when clicking outside
    window.addEventListener('click', function(event) {
        if (event.target.classList.contains('modal')) {
            closeModal(event.target);
        }
    });
    
    // Close modal with Escape key
    document.addEventListener('keydown', function(event) {
        if (event.key === 'Escape') {
            const openModal = document.querySelector('.modal.active');
            if (openModal) {
                closeModal(openModal);
            }
        }
    });
}

function openModal(modal) {
    if (modal) {
        modal.classList.add('active');
        document.body.style.overflow = 'hidden';
    }
}

function closeModal(modal) {
    if (modal) {
        modal.classList.remove('active');
        document.body.style.overflow = 'auto';
        
        // Reset form if it's a password modal
        const form = modal.querySelector('form');
        if (form) {
            form.reset();
        }
    }
}

// Form handling
function initForms() {
    const profileForm = document.querySelector('.profile-form');
    const passwordForm = document.querySelector('.password-form');
    
    // Profile form submission
    if (profileForm) {
        profileForm.addEventListener('submit', function(e) {
            e.preventDefault();
            saveProfileChanges(this);
        });
    }
    
    // Password form submission
    if (passwordForm) {
        passwordForm.addEventListener('submit', function(e) {
            e.preventDefault();
            updatePassword(this);
        });
    }
    
    // Form cancel buttons
    const cancelButtons = document.querySelectorAll('.btn-secondary');
    cancelButtons.forEach(btn => {
        if (!btn.classList.contains('close-modal')) {
            btn.addEventListener('click', function() {
                const form = this.closest('form');
                if (form) {
                    form.reset();
                    // Reset to original values for profile form
                    if (form.classList.contains('profile-form')) {
                        resetProfileForm();
                    }
                }
            });
        }
    });
}

function saveProfileChanges(form) {
    const formData = new FormData(form);
    const data = Object.fromEntries(formData);
    
    // Save original button text
    const submitBtn = form.querySelector('button[type="submit"]');
    if (submitBtn && !submitBtn.getAttribute('data-original-text')) {
        submitBtn.setAttribute('data-original-text', submitBtn.innerHTML);
    }
    
    // Simulate API call
    showLoadingState(form, true);
    
    setTimeout(() => {
        showLoadingState(form, false);
        
        // Show success message
        showNotification('Profile updated successfully!', 'success');
        
        // Update user name in header if full name was changed
        const fullNameInput = document.getElementById('fullName');
        if (fullNameInput) {
            const userNameElements = document.querySelectorAll('.user-name, .header-user span');
            userNameElements.forEach(element => {
                element.textContent = fullNameInput.value;
            });
        }
        
        console.log('Profile data saved:', data);
    }, 1500);
}

function updatePassword(form) {
    const formData = new FormData(form);
    const data = Object.fromEntries(formData);
    
    // Basic validation
    if (data.newPassword !== data.confirmPassword) {
        showNotification('New passwords do not match!', 'error');
        return;
    }
    
    if (data.newPassword.length < 6) {
        showNotification('Password must be at least 6 characters long!', 'error');
        return;
    }
    
    // Save original button text
    const submitBtn = form.querySelector('button[type="submit"]');
    if (submitBtn && !submitBtn.getAttribute('data-original-text')) {
        submitBtn.setAttribute('data-original-text', submitBtn.innerHTML);
    }
    
    // Simulate API call
    showLoadingState(form, true);
    
    setTimeout(() => {
        showLoadingState(form, false);
        
        // Show success message
        showNotification('Password updated successfully!', 'success');
        
        // Close modal
        const modal = form.closest('.modal');
        closeModal(modal);
        
        console.log('Password updated:', data);
    }, 1500);
}

function resetProfileForm() {
    // Reset form to original values
    const form = document.querySelector('.profile-form');
    if (form) {
        form.reset();
        showNotification('Changes discarded', 'info');
    }
}

function showLoadingState(form, isLoading) {
    const submitBtn = form.querySelector('button[type="submit"]');
    const cancelBtn = form.querySelector('.btn-secondary');
    
    if (isLoading) {
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Saving...';
        if (cancelBtn) cancelBtn.disabled = true;
    } else {
        submitBtn.disabled = false;
        const originalText = submitBtn.getAttribute('data-original-text') || 'Save Changes';
        submitBtn.innerHTML = originalText;
        if (cancelBtn) cancelBtn.disabled = false;
    }
}

// Password visibility toggle
function initPasswordToggle() {
    const toggleButtons = document.querySelectorAll('.toggle-password');
    
    toggleButtons.forEach(button => {
        button.addEventListener('click', function() {
            const passwordInput = this.previousElementSibling;
            const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
            passwordInput.setAttribute('type', type);
            
            // Toggle icon
            this.classList.toggle('fa-eye');
            this.classList.toggle('fa-eye-slash');
        });
    });
}

// Status toggle functionality
function initStatusToggle() {
    const statusButtons = document.querySelectorAll('.status-btn');
    
    statusButtons.forEach(button => {
        button.addEventListener('click', function() {
            const toggleGroup = this.parentElement;
            const buttons = toggleGroup.querySelectorAll('.status-btn');
            
            // Remove active class from all buttons in group
            buttons.forEach(btn => btn.classList.remove('active'));
            
            // Add active class to clicked button
            this.classList.add('active');
            
            // Update form data or show notification
            const status = this.getAttribute('data-status');
            if (status === 'moved') {
                showNotification('Status changed to Moved Out. This may affect your access.', 'warning');
            } else {
                showNotification('Status changed to Active', 'success');
            }
        });
    });
}

// Notification system
function initNotifications() {
    const notificationIcon = document.querySelector('.notification-icon');
    
    if (notificationIcon) {
        notificationIcon.addEventListener('click', function() {
            showNotification('No new notifications', 'info');
            
            // Simulate marking notifications as read
            const badge = this.querySelector('.notification-badge');
            if (badge) {
                badge.style.display = 'none';
            }
        });
    }
}

function showNotification(message, type = 'info') {
    // Remove existing notification
    const existingNotification = document.querySelector('.custom-notification');
    if (existingNotification) {
        existingNotification.remove();
    }
    
    // Create notification element
    const notification = document.createElement('div');
    notification.className = `custom-notification notification-${type}`;
    notification.innerHTML = `
        <div class="notification-content">
            <i class="fas fa-${getNotificationIcon(type)}"></i>
            <span>${message}</span>
        </div>
        <button class="notification-close"><i class="fas fa-times"></i></button>
    `;
    
    // Add styles
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${getNotificationColor(type)};
        color: white;
        padding: 1rem 1.5rem;
        border-radius: 8px;
        box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
        z-index: 3000;
        display: flex;
        align-items: center;
        gap: 0.75rem;
        max-width: 400px;
        animation: slideInRight 0.3s ease;
        font-family: 'Poppins', sans-serif;
    `;
    
    // Add close button styles
    const closeBtn = notification.querySelector('.notification-close');
    closeBtn.style.cssText = `
        background: none;
        border: none;
        color: white;
        cursor: pointer;
        padding: 0.25rem;
        border-radius: 4px;
        transition: background-color 0.3s;
    `;
    
    closeBtn.addEventListener('mouseenter', function() {
        this.style.backgroundColor = 'rgba(255, 255, 255, 0.2)';
    });
    
    closeBtn.addEventListener('mouseleave', function() {
        this.style.backgroundColor = 'transparent';
    });
    
    closeBtn.addEventListener('click', function() {
        notification.style.animation = 'slideOutRight 0.3s ease';
        setTimeout(() => notification.remove(), 300);
    });
    
    // Add keyframes for animation
    if (!document.querySelector('#notification-styles')) {
        const style = document.createElement('style');
        style.id = 'notification-styles';
        style.textContent = `
            @keyframes slideInRight {
                from {
                    transform: translateX(100%);
                    opacity: 0;
                }
                to {
                    transform: translateX(0);
                    opacity: 1;
                }
            }
            @keyframes slideOutRight {
                from {
                    transform: translateX(0);
                    opacity: 1;
                }
                to {
                    transform: translateX(100%);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
    }
    
    document.body.appendChild(notification);
    
    // Auto remove after 5 seconds
    setTimeout(() => {
        if (notification.parentElement) {
            notification.style.animation = 'slideOutRight 0.3s ease';
            setTimeout(() => notification.remove(), 300);
        }
    }, 5000);
}

function getNotificationIcon(type) {
    const icons = {
        success: 'check-circle',
        error: 'exclamation-circle',
        warning: 'exclamation-triangle',
        info: 'info-circle'
    };
    return icons[type] || 'info-circle';
}

function getNotificationColor(type) {
    const colors = {
        success: 'linear-gradient(135deg, #27ae60, #219a52)',
        error: 'linear-gradient(135deg, #e74c3c, #c0392b)',
        warning: 'linear-gradient(135deg, #f39c12, #e67e22)',
        info: 'linear-gradient(135deg, #3498db, #2980b9)'
    };
    return colors[type] || colors.info;
}

// Date and time functionality
function updateDateTime() {
    const dateDisplay = document.querySelector('.date-display p');
    if (dateDisplay) {
        const now = new Date();
        const options = { 
            weekday: 'long', 
            year: 'numeric', 
            month: 'long', 
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        };
        const formattedDate = now.toLocaleDateString('en-US', options);
        dateDisplay.innerHTML = `<i class="fas fa-clock"></i> ${formattedDate}`;
    }
}

// Payment functionality
function initPayment() {
    const instapayButton = document.querySelector('.instapay-button');
    
    if (instapayButton) {
        instapayButton.addEventListener('click', function() {
            // Simulate payment processing
            showNotification('Redirecting to InstaPay...', 'info');
            
            setTimeout(() => {
                showNotification('Payment processed successfully!', 'success');
                
                // Update UI to reflect payment
                const dueAmount = document.querySelector('.payment-amount.due');
                const paidAmount = document.querySelector('.payment-amount.paid');
                const statValue = document.querySelector('.stat-card:first-child .stat-value');
                
                if (dueAmount && paidAmount && statValue) {
                    dueAmount.textContent = '₱0';
                    paidAmount.textContent = '₱14,000';
                    statValue.textContent = '₱0';
                    
                    // Update status
                    const paymentStatus = document.querySelector('.payment-detail p');
                    if (paymentStatus) {
                        paymentStatus.innerHTML = '<strong>Next Due:</strong> 29 December 2025';
                    }
                }
            }, 2000);
        });
    }
}

// Export functions for global access (if needed)
window.TenantDashboard = {
    showNotification,
    openModal,
    closeModal,
    updateDateTime
};

// Payment functionality
function initPayment() {
    const instapayButton = document.querySelector('.instapay-button');
    const paymentMethods = document.querySelectorAll('.payment-method');
    const paymentForm = document.querySelector('.payment-form');
    
    // Payment method selection
    paymentMethods.forEach(method => {
        method.addEventListener('click', function() {
            paymentMethods.forEach(m => m.classList.remove('active'));
            this.classList.add('active');
        });
    });
    
    // Payment form submission
    if (paymentForm) {
        paymentForm.addEventListener('submit', function(e) {
            e.preventDefault();
            processPayment(this);
        });
    }
    
    // Download receipt
    const downloadBtn = document.querySelector('.btn-download');
    if (downloadBtn) {
        downloadBtn.addEventListener('click', function() {
            downloadReceipt();
        });
    }
    
    // Existing InstaPay button functionality
    if (instapayButton) {
        instapayButton.addEventListener('click', function() {
            showNotification('Redirecting to InstaPay...', 'info');
            
            setTimeout(() => {
                showNotification('Payment processed successfully!', 'success');
                updatePaymentUI();
            }, 2000);
        });
    }
}

function processPayment(form) {
    const formData = new FormData(form);
    const data = Object.fromEntries(formData);
    
    // Basic validation
    if (!data.referenceNo) {
        showNotification('Please enter a reference number!', 'error');
        return;
    }
    
    showLoadingState(form, true);
    
    setTimeout(() => {
        showLoadingState(form, false);
        showNotification('Payment submitted successfully!', 'success');
        
        // Update payment history
        updatePaymentHistory(data);
        
        // Reset form
        form.reset();
        
        console.log('Payment data:', data);
    }, 2000);
}

function updatePaymentHistory(paymentData) {
    const paymentHistory = document.querySelector('.payment-history-table');
    
    // Create new row
    const newRow = document.createElement('div');
    newRow.className = 'table-row';
    newRow.innerHTML = `
        <div class="table-cell">${new Date().toLocaleDateString()}</div>
        <div class="table-cell">Rent</div>
        <div class="table-cell">₱3,500</div>
        <div class="table-cell">InstaPay</div>
        <div class="table-cell">November 2025</div>
        <div class="table-cell"><span class="status-badge paid">Paid</span></div>
    `;
    
    // Insert at the beginning (after header)
    paymentHistory.appendChild(newRow);
}

function downloadReceipt() {
    showNotification('Generating receipt...', 'info');
    
    setTimeout(() => {
        showNotification('Receipt downloaded successfully!', 'success');
        // In a real app, this would trigger a file download
        console.log('Receipt download triggered');
    }, 1000);
}

function updatePaymentUI() {
    const dueAmount = document.querySelector('.payment-amount.due');
    const paidAmount = document.querySelector('.payment-amount.paid');
    const statValue = document.querySelector('.stat-card:first-child .stat-value');
    
    if (dueAmount && paidAmount && statValue) {
        dueAmount.textContent = '₱0';
        paidAmount.textContent = '₱14,000';
        statValue.textContent = '₱0';
        
        const paymentStatus = document.querySelector('.payment-detail p');
        if (paymentStatus) {
            paymentStatus.innerHTML = '<strong>Next Due:</strong> 29 December 2025';
        }
    }
}

// Request/Complaint functionality
function initRequests() {
    const requestForm = document.querySelector('.request-form');
    const urgencyLevels = document.querySelectorAll('.urgency-level');
    const clearBtn = document.querySelector('.btn-clear');
    
    // Urgency level selection
    urgencyLevels.forEach(level => {
        level.addEventListener('click', function() {
            urgencyLevels.forEach(l => l.classList.remove('active'));
            this.classList.add('active');
        });
    });
    
    // Request form submission
    if (requestForm) {
        requestForm.addEventListener('submit', function(e) {
            e.preventDefault();
            submitRequest(this);
        });
    }
    
    // Clear form
    if (clearBtn) {
        clearBtn.addEventListener('click', function() {
            const form = this.closest('form');
            if (form) {
                form.reset();
                urgencyLevels.forEach(l => l.classList.remove('active'));
                urgencyLevels[1].classList.add('active'); // Set medium as default
                showNotification('Form cleared!', 'info');
            }
        });
    }
    
    // Set current date as default for date filed
    const dateField = document.getElementById('dateFiled');
    if (dateField) {
        const today = new Date().toISOString().split('T')[0];
        dateField.value = today;
    }
}

function submitRequest(form) {
    const formData = new FormData(form);
    const data = Object.fromEntries(formData);
    
    // Get selected urgency level
    const urgencyLevel = document.querySelector('.urgency-level.active');
    data.urgency = urgencyLevel ? urgencyLevel.getAttribute('data-level') : 'medium';
    
    // Basic validation
    if (!data.issueCategory || !data.issueDescription) {
        showNotification('Please fill in all required fields!', 'error');
        return;
    }
    
    showLoadingState(form, true);
    
    setTimeout(() => {
        showLoadingState(form, false);
        showNotification('Request submitted successfully!', 'success');
        
        // Update requests table
        updateRequestsTable(data);
        
        // Update statistics
        updateRequestStats();
        
        // Reset form
        form.reset();
        const urgencyLevels = document.querySelectorAll('.urgency-level');
        urgencyLevels.forEach(l => l.classList.remove('active'));
        urgencyLevels[1].classList.add('active'); // Set medium as default
        
        console.log('Request data:', data);
    }, 1500);
}

function updateRequestsTable(requestData) {
    const requestsTable = document.querySelector('.requests-table');
    const requestId = `#REQ-${Math.floor(Math.random() * 1000).toString().padStart(3, '0')}`;
    
    // Create new row
    const newRow = document.createElement('div');
    newRow.className = 'table-row';
    newRow.innerHTML = `
        <div class="table-cell">${requestId}</div>
        <div class="table-cell">${requestData.issueCategory}</div>
        <div class="table-cell">${new Date().toLocaleDateString()}</div>
        <div class="table-cell"><span class="status-badge pending">Pending</span></div>
        <div class="table-cell">${requestData.issueDescription.substring(0, 50)}...</div>
    `;
    
    // Insert at the beginning (after header)
    const tableHeader = requestsTable.querySelector('.table-header');
    requestsTable.insertBefore(newRow, tableHeader.nextSibling);
}

function updateRequestStats() {
    const totalRequests = document.querySelector('.request-stat:first-child .stat-value');
    const pendingRequests = document.querySelector('.request-stat:nth-child(2) .stat-value');
    
    if (totalRequests && pendingRequests) {
        const currentTotal = parseInt(totalRequests.textContent);
        const currentPending = parseInt(pendingRequests.textContent);
        
        totalRequests.textContent = (currentTotal + 1).toString();
        pendingRequests.textContent = (currentPending + 1).toString();
    }
}

// Update the initDashboard function to include new initializers
function initDashboard() {
    // Initialize all components
    initSidebar();
    initNavigation();
    initModals();
    initForms();
    initNotifications();
    initPasswordToggle();
    initStatusToggle();
    initPayment();
    initRequests();
    updateDateTime();
    
    // Set up periodic updates
    setInterval(updateDateTime, 60000);
}