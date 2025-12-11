// Transaction Management JavaScript

$(document).ready(function () {
    // Category Filter
    $('#categoryFilter').on('change', function () {
        var selectedCategoryId = $(this).val();

        if (selectedCategoryId === '') {
            // Show all transactions
            $('.list-row').show();
        } else {
            // Hide all rows first
            $('.list-row').hide();

            // Show only rows matching the selected category
            $('.list-row').each(function () {
                var rowCategoryId = $(this).find('td:eq(4)').text().trim();
                if (rowCategoryId === selectedCategoryId) {
                    $(this).show();
                }
            });
        }
    });

    // Delete Transaction Modal
    $('.openDeleteTransactionModalBtn').on('click', function () {
        var row = $(this).closest('tr');
        var transactionId = row.find('td:eq(0)').text().trim();

        $('#deleteTransactionId').val(transactionId);

        var deleteModal = new bootstrap.Modal(document.getElementById('deleteTransactionModal'));
        deleteModal.show();
    });

    // Confirm Delete
    $('#confirmDeleteBtn').on('click', function () {
        var transactionId = $('#deleteTransactionId').val();

        $.ajax({
            url: '/Home/DeleteTransaction',
            type: 'POST',
            data: { id: transactionId },
            success: function (response) {
                if (response.success) {
                    location.reload();
                } else {
                    alert('Error deleting transaction: ' + response.message);
                }
            },
            error: function () {
                alert('Error deleting transaction');
            }
        });
    });

    // Update Transaction Modal
    $('.openUpdateTransactionModalBtn').on('click', function () {
        var row = $(this).closest('tr');

        var transactionId = row.find('td:eq(0)').text().trim();
        var date = row.find('.transaction-date').text().trim();
        var name = row.find('.transaction-name').text().trim();
        var transactionType = row.find('td:eq(5)').text().trim();
        var categoryId = row.find('td:eq(4)').text().trim();
        var amountText = row.find('.transaction-amount').text().trim();

        // Extract amount (remove + or - and $)
        var amount = amountText.replace(/[+\-$]/g, '').trim();

        // Convert date format from DD/MM/YYYY HH:MM:SS to YYYY-MM-DD
        var dateParts = date.split(' ')[0].split('/');
        var formattedDate = dateParts[2] + '-' + dateParts[1] + '-' + dateParts[0];

        $('#updateTransactionId').val(transactionId);
        $('#updateName').val(name);
        $('#updateDate').val(formattedDate);
        $('#updateAmount').val(amount);
        $('#updateTransactionType').val(transactionType);
        $('#updateCategoryId').val(categoryId);

        var updateModal = new bootstrap.Modal(document.getElementById('updateTransactionModal'));
        updateModal.show();
    });

    // Confirm Update
    $('#confirmUpdateBtn').on('click', function () {
        var transactionData = {
            id: $('#updateTransactionId').val(),
            name: $('#updateName').val(),
            date: $('#updateDate').val(),
            amount: $('#updateAmount').val(),
            transactionType: $('#updateTransactionType').val(),
            categoryId: $('#updateCategoryId').val()
        };

        $.ajax({
            url: '/Home/UpdateTransaction',
            type: 'POST',
            data: transactionData,
            success: function (response) {
                if (response.success) {
                    location.reload();
                } else {
                    alert('Error updating transaction: ' + response.message);
                }
            },
            error: function () {
                alert('Error updating transaction');
            }
        });
    });

    // Add Transaction Modal
    $('#openAddTransactionModalBtn').on('click', function () {
        // Set today's date as default
        var today = new Date().toISOString().split('T')[0];
        $('#addDate').val(today);

        // Clear form
        $('#addName').val('');
        $('#addAmount').val('');
        $('#addTransactionType').val('0'); // Default to Expense

        var addModal = new bootstrap.Modal(document.getElementById('addTransactionModal'));
        addModal.show();
    });

    // Confirm Add
    $('#confirmAddBtn').on('click', function () {
        var transactionData = {
            name: $('#addName').val(),
            date: $('#addDate').val(),
            amount: $('#addAmount').val(),
            transactionType: $('#addTransactionType').val(),
            categoryId: $('#addCategoryId').val()
        };

        $.ajax({
            url: '/Home/AddTransaction',
            type: 'POST',
            data: transactionData,
            success: function (response) {
                if (response.success) {
                    location.reload();
                } else {
                    alert('Error adding transaction: ' + response.message);
                }
            },
            error: function () {
                alert('Error adding transaction');
            }
        });
    });
});
