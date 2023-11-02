import * as Yup from 'yup';
export const validationSchema = Yup.object({
    code: Yup.string()
        .trim()
        .max(50, 'The Code must be 50 characters!')
        .required('The Code field is required!'),
    name: Yup.string()
        .trim()
        .max(200, 'The Name must be 200 characters!')
        .required('The Name field is required!'),
    department: Yup.string().required('The Deparment field is required!')
});