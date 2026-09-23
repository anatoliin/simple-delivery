BEGIN TRANSACTION;

DELETE FROM Orders;
DELETE FROM sqlite_sequence WHERE name = 'Orders';

INSERT INTO Orders (
    OrderNumber,
    SenderCity,
    SenderAddress,
    RecipientCity,
    RecipientAddress,
    Weight,
    PickupDate,
    CreatedAt
)
VALUES ('DLV-000001', 'Москва', 'ул. Тверская, 12', 'Казань', 'ул. Баумана, 8', 2.5, '2026-09-24', '2026-09-23T08:00:00Z');

INSERT INTO Orders (OrderNumber, SenderCity, SenderAddress, RecipientCity, RecipientAddress, Weight, PickupDate, CreatedAt) SELECT 'DLV-000002', 'Санкт-Петербург', 'Невский проспект, 28', 'Москва', 'ул. Арбат, 15', 7.2, '2026-09-25', '2026-09-23T08:10:00Z' WHERE NOT EXISTS (SELECT 1 FROM Orders WHERE OrderNumber = 'DLV-000002');
INSERT INTO Orders (OrderNumber, SenderCity, SenderAddress, RecipientCity, RecipientAddress, Weight, PickupDate, CreatedAt) SELECT 'DLV-000003', 'Казань', 'ул. Кремлевская, 4', 'Самара', 'ул. Ленинградская, 21', 1.8, '2026-09-26', '2026-09-23T08:20:00Z' WHERE NOT EXISTS (SELECT 1 FROM Orders WHERE OrderNumber = 'DLV-000003');
INSERT INTO Orders (OrderNumber, SenderCity, SenderAddress, RecipientCity, RecipientAddress, Weight, PickupDate, CreatedAt) SELECT 'DLV-000004', 'Екатеринбург', 'ул. Малышева, 36', 'Пермь', 'ул. Ленина, 52', 12.0, '2026-09-27', '2026-09-23T08:30:00Z' WHERE NOT EXISTS (SELECT 1 FROM Orders WHERE OrderNumber = 'DLV-000004');
INSERT INTO Orders (OrderNumber, SenderCity, SenderAddress, RecipientCity, RecipientAddress, Weight, PickupDate, CreatedAt) SELECT 'DLV-000005', 'Новосибирск', 'Красный проспект, 25', 'Омск', 'ул. Гагарина, 10', 4.4, '2026-09-28', '2026-09-23T08:40:00Z' WHERE NOT EXISTS (SELECT 1 FROM Orders WHERE OrderNumber = 'DLV-000005');
INSERT INTO Orders (OrderNumber, SenderCity, SenderAddress, RecipientCity, RecipientAddress, Weight, PickupDate, CreatedAt) SELECT 'DLV-000006', 'Ростов-на-Дону', 'ул. Большая Садовая, 17', 'Краснодар', 'ул. Красная, 44', 9.6, '2026-09-29', '2026-09-23T08:50:00Z' WHERE NOT EXISTS (SELECT 1 FROM Orders WHERE OrderNumber = 'DLV-000006');
INSERT INTO Orders (OrderNumber, SenderCity, SenderAddress, RecipientCity, RecipientAddress, Weight, PickupDate, CreatedAt) SELECT 'DLV-000007', 'Уфа', 'ул. Ленина, 9', 'Челябинск', 'проспект Победы, 61', 3.1, '2026-09-30', '2026-09-23T09:00:00Z' WHERE NOT EXISTS (SELECT 1 FROM Orders WHERE OrderNumber = 'DLV-000007');
INSERT INTO Orders (OrderNumber, SenderCity, SenderAddress, RecipientCity, RecipientAddress, Weight, PickupDate, CreatedAt) SELECT 'DLV-000008', 'Воронеж', 'ул. Плехановская, 18', 'Тула', 'проспект Ленина, 33', 6.7, '2026-10-01', '2026-09-23T09:10:00Z' WHERE NOT EXISTS (SELECT 1 FROM Orders WHERE OrderNumber = 'DLV-000008');
INSERT INTO Orders (OrderNumber, SenderCity, SenderAddress, RecipientCity, RecipientAddress, Weight, PickupDate, CreatedAt) SELECT 'DLV-000009', 'Нижний Новгород', 'ул. Большая Покровская, 7', 'Иваново', 'ул. Советская, 14', 2.2, '2026-10-02', '2026-09-23T09:20:00Z' WHERE NOT EXISTS (SELECT 1 FROM Orders WHERE OrderNumber = 'DLV-000009');
INSERT INTO Orders (OrderNumber, SenderCity, SenderAddress, RecipientCity, RecipientAddress, Weight, PickupDate, CreatedAt) SELECT 'DLV-000010', 'Владивосток', 'ул. Светланская, 39', 'Хабаровск', 'ул. Муравьева-Амурского, 16', 15.5, '2026-10-03', '2026-09-23T09:30:00Z' WHERE NOT EXISTS (SELECT 1 FROM Orders WHERE OrderNumber = 'DLV-000010');

COMMIT;