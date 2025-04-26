window.printBill = function (billData) {
    const billElement = document.getElementById('bill-section');

    const newWindow = window.open('', '_blank');
    newWindow.document.write('<html><head><title>Bill</title>');
    newWindow.document.write('<style>');
    newWindow.document.write(`
        body {
            border: 1px solid black;
            font-family: Arial, sans-serif;
        }
        h1, h4 {
            text-align: center;
        }
        table, th, td {
            border: 1px solid black;
            border-collapse: collapse;
            padding: 5px;
        }
        table {
            width: 100%;
        }
        .info-section {
            display: flex;
            justify-content: space-evenly;
            align-items: center;
            border: 1px solid black;
            padding: 10px;
        }
    `);
    newWindow.document.write('</style></head><body>');



    newWindow.document.write('<div style="display: flex; border: 1px solid black; justify-content: center; align-items: center;">');
    newWindow.document.write('<h1 >' + billData.tenantName + '</h1>');
    newWindow.document.write('</div>');
    newWindow.document.write('<div style="display: flex; border: 1px solid black; justify-content: space-evenly; align-items: center;">');
    newWindow.document.write('<p>' + billData.tenantAddress + '</p>');
    newWindow.document.write('<p>Phone: ' + billData.tenantPhone + '</p>');
    newWindow.document.write('</div>');
    newWindow.document.write('<h4  >Sale Info</h4>');
    newWindow.document.write('<div style="display: flex; border: 1px solid black; justify-content: space-evenly; align-items: center;">');
    newWindow.document.write('<p><strong>Bill No:</strong> ' + billData.saleId + '</p>');
    newWindow.document.write('<p><strong>Date:</strong> ' + billData.saleDate + '</p>');
    newWindow.document.write('</div>');
    if (billData.customerName) {
        newWindow.document.write('<h4>Customer Info</h4>');
        newWindow.document.write('<div style="display: flex; justify-content: space-between; align-items: center;">');
        newWindow.document.write('<p><strong>Customer Name:</strong> ' + billData.customerName + '</p>');

        if (billData.customerReference) {
            newWindow.document.write('<p><strong>Reference:</strong> ' + billData.customerReference + '</p>');
        }
        newWindow.document.write('</div>');
    }

    newWindow.document.write('<h4>Items</h4>');
    newWindow.document.write('<table style="width: 100%; border-collapse: collapse;">');
    newWindow.document.write('<thead><tr><th style="border: 1px solid black;">Sr.</th><th style="border: 1px solid black;">Name</th><th style="border: 1px solid black;">Qty</th><th style="border: 1px solid black;" >Qty Unit</th><th style="border: 1px solid black;" >Rate</th><th style="border: 1px solid black;">Amount</th></tr></thead><tbody>');

    billData.products.forEach(function (product, index) {
        newWindow.document.write('<tr>');
        newWindow.document.write('<td style="border: 1px solid black; padding: 5px; text-align: center;">' + (index + 1) + '</td>');
        newWindow.document.write('<td style="border: 1px solid black; padding: 5px;">' + product.name + '</td>');
        newWindow.document.write('<td style="border: 1px solid black; padding: 5px; text-align: center;">' + product.quantity + '</td>');
        newWindow.document.write('<td style="border: 1px solid black; padding: 5px; text-align: center;">' + product.quantityUnit + '</td>');
        newWindow.document.write('<td style="border: 1px solid black; padding: 5px; text-align: right;">' + product.rate + '</td>');
        newWindow.document.write('<td style="border: 1px solid black; padding: 5px; text-align: right;">' + product.amount + '</td>');
        newWindow.document.write('</tr>');
    });

    newWindow.document.write('</tbody></table>');
    newWindow.document.write(`
  <div style="
    display: flex;
    justify-content: flex-end;
    border: 1px solid black;
    width: 10.65rem;
    margin: 0 0 0 auto;
    padding: 10px;">
    <strong>Discount:</strong> ${billData.discount}
  </div>
`);

newWindow.document.write('<h1 style="display:flex; justify-content:center;">Total Amount: ' + billData.totalAmount + '</h1>');
    newWindow.document.write('<div style=" width: 100%; text-align: center;">');
    newWindow.document.write('<p>Thank you for your business!</p>');
    newWindow.document.write('<p>Please visit us again.</p>');
    newWindow.document.write('</div>');
    newWindow.document.write('</body></html>');
    newWindow.document.close();
    newWindow.print();
};
window.printSupplierBill = function (billData) {
    const newWindow = window.open('', '_blank');
    newWindow.document.write('<html><head><title>Bill</title>');
    newWindow.document.write('<style>');
    newWindow.document.write(`
        body {
            border: 1px solid black;
            font-family: Arial, sans-serif;
        }
        h1, h4 {
            text-align: center;
        }
        table, th, td {
            border: 1px solid black;
            border-collapse: collapse;
            padding: 5px;
        }
        table {
            width: 100%;
        }
        .info-section {
            display: flex;
            justify-content: space-evenly;
            align-items: center;
            border: 1px solid black;
            padding: 10px;
        }
    `);
    newWindow.document.write('</style></head><body>');

    // Header with tenant details
   
    newWindow.document.write('<h1 style="text-align: center; ">' + billData.tenantName + '</h1>');
    newWindow.document.write('<div style="border: 1px solid black; display: flex; justify-content: space-evenly;">'); newWindow.document.write('<p><strong>Location:</strong>' + billData.tenantAddress + '</p>');
    newWindow.document.write('<p><strong>Contact No:</strong>: ' + billData.tenantPhone + '</p>');
    newWindow.document.write('</div>');

    // Sale information
    newWindow.document.write('<h4 style=" text-align:center;">Bill Info</h4>');
    newWindow.document.write('<div style=" border:1px solid black; display: flex; justify-content: space-evenly;">');
    newWindow.document.write('<p><strong>Bill No:</strong> ' + billData.saleId + '</p>');
    newWindow.document.write('<p><strong>Date:</strong> ' + billData.saleDate + '</p>');
    newWindow.document.write('</div>');

  

    // Items table
    newWindow.document.write('<h4>Items</h4>');
    newWindow.document.write('<table>');
    newWindow.document.write('<thead><tr><th>Sr.</th><th>Name</th><th>Qty</th><th>Qty Unit</th><th>Rate</th><th>Amount</th></tr></thead><tbody>');

    billData.products.forEach(function (product, index) {
        newWindow.document.write('<tr style="border: 1px solid black; padding: 5px; text-align: center;">');
        newWindow.document.write('<td style="border: 1px solid black; padding: 5px; text-align: center;">' + (index + 1) + '</td>');
        newWindow.document.write('<td style="border: 1px solid black; padding: 5px; text-align: center;">' + product.name + '</td>');
        newWindow.document.write('<td style="border: 1px solid black; padding: 5px; text-align: center;">' + product.quantity + '</td>');
        newWindow.document.write('<td style="border: 1px solid black; padding: 5px; text-align: center;">' + product.quantityUnit + '</td>');
        newWindow.document.write('<td style="border: 1px solid black; padding: 5px; text-align: center;">' + product.rate + '</td>');
        newWindow.document.write('<td style="border: 1px solid black; padding: 5px; text-align: center;">' + product.amount + '</td>');
        newWindow.document.write('</tr>');
    });

    newWindow.document.write('</tbody></table>');

   

    // Total Amount
    newWindow.document.write('<h1 style="text-align:center; font-size: 24px;">Total Amount: ' + billData.totalAmount + '</h1>');

    // Footer
    newWindow.document.write('<div style="text-align: center;">');
    newWindow.document.write('<p>Thank you for your business!</p>'); 
    newWindow.document.write('</div>');

    newWindow.document.write('</body></html>');
    newWindow.document.close();
    newWindow.print();
};
function setCookie(name, value, days) {
    const date = new Date();
    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
    const expires = "expires=" + date.toUTCString();
    document.cookie = name + "=" + value + ";" + expires + ";path=/";
}